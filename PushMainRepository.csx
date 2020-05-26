#load "Shared/All.csx"

public string CommitMessage = "Auto commit";

if(Args.Count > 0)
{
    CommitMessage = Args[0];
}

Console.WriteLine("Starting CommunicatorCMS push.");
Console.WriteLine("");

Console.WriteLine("Renaming all .git to ._git");        
GitActions.RenameAllGitDirectories(Global.CurrentDirectory);

Console.WriteLine("Renaming CommunicatorCMS ._git to .git");
GitActions.RenameGitAltDirectory(Global.CurrentDirectory);

Console.WriteLine("git add -A && git commit -m <COMMIT MESSAGE> && git push");
Console.WriteLine("");

var process = await GitActions.AddCommitAndPush(Global.CurrentDirectory, CommitMessage);

Console.WriteLine(process.StandardOutput.Trim() + "\n" + process.StandardError.Trim());

Console.WriteLine("Renaming all ._git to .git");                
GitActions.RenameAllGitAltDirectories(Global.CurrentDirectory);

Console.WriteLine("");
Console.WriteLine("CommunicatorCMS push complete.");
