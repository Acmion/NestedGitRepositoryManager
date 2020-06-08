public static async Task<Process> RunAsync(this Process process)
{
    var taskCompletionSource = new TaskCompletionSource<object>();
    process.EnableRaisingEvents = true;
    process.Exited += (s, e) => taskCompletionSource.TrySetResult(null);;

    if (!process.Start()) 
    {
        taskCompletionSource.SetException(new Exception("Failed to start process."));
    }
    
    await taskCompletionSource.Task;

    return process;
}

public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> func)
{
    foreach (var value in list)
    {
        await func(value);
    }
}