#load "Shared/All.csx"

Console.WriteLine("Pulling all sub repositories.");
Console.WriteLine("");                

var gitDirs = GitActions.GetAllGitDirectories(Global.CurrentDirectory).OrderBy(dir => dir).Where(dir => dir != Global.CurrentDirectory);

if(!gitDirs.Any())
{
    Console.WriteLine("No sub repositories found.");
    return;
}
else
{
    var pullableDirectories = new List<string>();

var parentDir = gitDirs.First();
pullableDirectories.Add(parentDir);

    foreach(var gitDir in gitDirs)
    {
        if(!gitDir.StartsWith(parentDir))
        {
            parentDir = gitDir;
            pullableDirectories.Add(parentDir);
        }
    }

    Console.WriteLine("Renaming all .git to ._git");
    Console.WriteLine("");

    GitActions.RenameAllGitDirectories(Global.CurrentDirectory);

    var tasks = new List<Task>();

    foreach (var dir in pullableDirectories)
    {
        var task = Task.Run(async () =>
        {
            GitActions.RenameGitAltDirectory(dir);

            var process = await GitActions.Pull(dir);

            var consoleOutputLines = new List<string>();
            consoleOutputLines.Add(dir);
            consoleOutputLines.Add("Renaming ._git to .git");
            consoleOutputLines.Add("git pull");
            consoleOutputLines.Add(process.StandardOutput.Trim() + "\n" + process.StandardError.Trim());
            consoleOutputLines.Add("\n");

            Console.Write(string.Join("\n", consoleOutputLines));
        });

tasks.Add(task);
    }

    foreach (var task in tasks)
    {
        await task;
    }
}

Console.WriteLine("Renaming all ._git to .git");

GitActions.RenameAllGitAltDirectories(Global.CurrentDirectory);

Console.WriteLine("Completed pulling all sub repositories");
Console.WriteLine("");