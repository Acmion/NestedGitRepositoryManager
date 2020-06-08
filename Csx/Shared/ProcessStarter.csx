public static async Task<(string StandardOutput, string StandardError)> StartProcess(string fileName, string arguments, string workingDirectory)
{
    var process = new Process();

    process.StartInfo.FileName = fileName;
    process.StartInfo.Arguments = arguments;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;
    process.StartInfo.WorkingDirectory = workingDirectory;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.EnableRaisingEvents = true;

    var output = new StringBuilder();
    var error = new StringBuilder();
    var taskCompletionSource = new TaskCompletionSource<object>();

    process.OutputDataReceived += (sender, eventArgs) => output.AppendLine(eventArgs.Data);
    process.ErrorDataReceived += (sender, eventArgs) => error.AppendLine(eventArgs.Data);
    process.Exited += (sender, eventArgs) => taskCompletionSource.TrySetResult(null);

    if (!process.Start())
    {
        taskCompletionSource.SetException(new Exception("Failed to start process."));
    }

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    await taskCompletionSource.Task;

    process.Dispose();

    return (output.ToString(), error.ToString());
}