#load "Global.csx"
#load "Extensions.csx"

public static class GitActions
{
    private static string GitDirectoryName { get; } = ".git";
    private static string GitAltDirectoryName { get; } = "._git";

    public static IEnumerable<string> GetAllGitDirectories(string path)
    {
        return Directory.GetDirectories(path, $"*{GitDirectoryName}", SearchOption.AllDirectories).Select(dir => dir.Replace(GitDirectoryName, ""));
    }

    public static IEnumerable<string> GetAllGitAltDirectories(string path)
    {
        return Directory.GetDirectories(path, $"*{GitAltDirectoryName}", SearchOption.AllDirectories).Select(dir => dir.Replace(GitAltDirectoryName, ""));
    }

    public static void RenameGitDirectory(string path)
    {
        var gitDir = Path.Combine(path, GitDirectoryName);

        if (Directory.Exists(gitDir))
        {
            Directory.Move(gitDir, gitDir.Replace(GitDirectoryName, GitAltDirectoryName));
        }
    }

    public static void RenameGitAltDirectory(string path)
    {
        var gitAltDir = Path.Combine(path, GitAltDirectoryName);

        if (Directory.Exists(gitAltDir))
        {
            Directory.Move(gitAltDir, gitAltDir.Replace(GitAltDirectoryName, GitDirectoryName));
        }
    }

    public static void RenameAllGitDirectories(string path)
    {
        foreach (var dir in GetAllGitDirectories(path))
        {
            RenameGitDirectory(dir);
        }
    }

    public static void RenameAllGitAltDirectories(string path)
    {
        foreach (var dir in GetAllGitAltDirectories(path))
        {
            RenameGitAltDirectory(dir);
        }
    }

    public static async Task<(string StandardOutput, string StandardError)> AddCommitAndPush(string path, string commitMessage)
    {
        var addAll = await AddAll(path);

        var commit = await Commit(path, commitMessage);

        var push = await Push(path);

        var joinedStandardOutput = string.Join("\n", addAll.StandardOutput, commit.StandardOutput, push.StandardOutput);
        var joinedStandardError = string.Join("\n", addAll.StandardError, commit.StandardError, push.StandardError);

        return (joinedStandardOutput, joinedStandardError);

    }

    public static async Task<(string StandardOutput, string StandardError)> AddAll(string path)
    {
        return await StartGitProcess(path, "add -A");
    }

    public static async Task<(string StandardOutput, string StandardError)> Commit(string path, string commitMessage)
    {
        return await StartGitProcess(path, $"commit -m \"{commitMessage}\" ");
    }

    public static async Task<(string StandardOutput, string StandardError)> Push(string path)
    {
        return await StartGitProcess(path, "push");
    }

    public static async Task<(string StandardOutput, string StandardError)> Pull(string path)
    {
        return await StartGitProcess(path, "pull");
    }

    public static async Task<(string StandardOutput, string StandardError)> RemoveCachedRecursively(string path)
    {
        return await StartGitProcess(path, "rm -r --cached " + path.Replace('\\', '/'));
    }

    public static async Task<(string StandardOutput, string StandardError)> StartGitProcess(string path, string arguments)
    {
        var process = new Process();

        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WorkingDirectory = path;
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
}