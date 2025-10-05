namespace Application.AppLogger;

public static class FileLogger
{
    private static readonly string RootPath = Path.Combine(@"D:\Repo\project_trotting\", "Logs");
    private static readonly string DriverNoPanelLog = Path.Combine(RootPath, "DriverNoPanelError.txt");
    public static readonly string HorseNoPanelLog = Path.Combine(RootPath, "HorseNoPanelError.txt");

    public static async Task AddToDriverNoPanel(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(DriverNoPanelLog, $"{sourceId} :: Driver no panel error\n");
    }

    public static async Task AddToHorseNoPanel(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(HorseNoPanelLog, $"{sourceId} :: Horse no panel error\n");
    }
}