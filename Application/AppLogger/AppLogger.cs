namespace Application;

public static class AppLogger
{
    public static void LogHeader(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        var formatted = $"  == {message} ==\n";
        Console.WriteLine(formatted);
        Console.ResetColor();
    }

    public static void LogSubheader(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        var formatted = $"  -- {message} --\n";
        Console.WriteLine(formatted);
        Console.ResetColor();
    }
    
    public static void LogNeutral(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        var formatted = $" [*] {message}";
        Console.WriteLine(formatted);
        Console.ResetColor();
    }
    
    public static void LogPositive(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        var formatted = $" [+] {message}";
        Console.WriteLine(formatted);
        Console.ResetColor();
    }

    public static void LogNegative(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        var formatted = $" [-] {message}";
        Console.WriteLine(formatted);
        Console.ResetColor();
    }
}