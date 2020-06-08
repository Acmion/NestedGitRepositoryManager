using System.Runtime.CompilerServices;

public static class Global
{
    public static string CurrentDirectory { get; } = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
}