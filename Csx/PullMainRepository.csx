#load "Shared/All.csx"

Console.WriteLine("Starting Main repository pull.");
Console.WriteLine("");

Console.WriteLine("Renaming all .git to ._git");                
GitActions.RenameAllGitDirectories(Global.CurrentDirectory);

Console.WriteLine("Renaming Main repository ._git to .git");
GitActions.RenameGitAltDirectory(Global.CurrentDirectory);

Console.WriteLine("git pull");
Console.WriteLine("");

var process = await GitActions.Pull(Global.CurrentDirectory);

Console.WriteLine(process.StandardOutput.Trim() + "\n" + process.StandardError.Trim());

Console.WriteLine("Renaming all ._git to .git");                
GitActions.RenameAllGitAltDirectories(Global.CurrentDirectory);

Console.WriteLine("");
Console.WriteLine("Main repository pull complete.");
