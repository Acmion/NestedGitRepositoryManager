#load "Shared/All.csx"

public string CommitMessage = "Auto commit";

if(Args.Count > 0)
{
    CommitMessage = Args[0];
}

Console.WriteLine("Adding, committing and pushing all sub repositories.");
Console.WriteLine("");                

GitActions.RenameAllGitAltDirectories(Global.CurrentDirectory);

var gitDirs = GitActions.GetAllGitDirectories(Global.CurrentDirectory).OrderBy(dir => dir).Where(dir => dir != Global.CurrentDirectory);

if(!gitDirs.Any())
{
    Console.WriteLine("No sub repositories found.");
    return;
}
else
{
    var directoryStacksByParentDirectory = new Dictionary<string, Stack<string>>();

var parentDir = gitDirs.First();
directoryStacksByParentDirectory[parentDir] = new Stack<string>();

    foreach(var gitDir in gitDirs)
    {
        if(!gitDir.StartsWith(parentDir))
        {
            parentDir = gitDir;
            directoryStacksByParentDirectory[parentDir] = new Stack<string>();
        }

        directoryStacksByParentDirectory[parentDir].Push(gitDir);
    }

    var tasks = new List<Task>();

    foreach (var dirs in directoryStacksByParentDirectory.Values)
    {
        var task = Task.Run(async () =>
        {
            await dirs.ForEachAsync(async dir =>
            {
                var process = await GitActions.AddCommitAndPush(dir, CommitMessage);

                GitActions.RenameGitDirectory(dir);

                var consoleOutputLines = new List<string>();
                consoleOutputLines.Add(dir);
                consoleOutputLines.Add("git add -A && git commit -m <COMMIT MESSAGE> && git push");
                consoleOutputLines.Add(process.StandardOutput.Trim() + "\n" + process.StandardError.Trim());
                consoleOutputLines.Add("");
                consoleOutputLines.Add("Renaming .git to ._git");
                consoleOutputLines.Add("\n");

                Console.Write(string.Join("\n", consoleOutputLines));
            });
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

Console.WriteLine("Completed pushing all sub repositories.");
Console.WriteLine("");