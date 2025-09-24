namespace Application.AppLogger;

public class AppLogger
{
    private readonly string[] _prefix =
    [
        "  [*]",
        "  [+]",
        "  [-]",
        " INFO",
        " WARN",
        "ERROR"
    ];

    /// <summary>
    /// Print to terminal with star prefix
    /// </summary>
    public void LogNeutral(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(_prefix[0]);
        Console.ResetColor();
        Console.WriteLine($" {message}");
    }
    
    public void LogPositive(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(_prefix[1]);
        Console.ResetColor();
        Console.WriteLine($" {message}");
    }

    public void LogNegative(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(_prefix[2]);
        Console.ResetColor();
        Console.WriteLine($" {message}");
    }
    
    public void LogInformation(string message, bool logToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(_prefix[3]);
        Console.ResetColor();
        Console.WriteLine($" : {message}");
    }

    public void LogWarning(string message, bool logToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(_prefix[4]);
        Console.ResetColor();
        Console.WriteLine($" : {message}");
    }

    public void LogError(string message, bool logToFile = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(_prefix[5]);
        Console.ResetColor();
        Console.WriteLine($" : {message}");
    }
}