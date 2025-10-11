namespace Application.AppLogger;

public static class FileLogger
{
    private static readonly string RootPath = Path.Combine(@"D:\Repo\project_trotting\", "Logs");
    private static readonly string DriverNoPanelLog = Path.Combine(RootPath, "DriverNoPanelError.txt");
    private static readonly string DriverNoYearSelectLog = Path.Combine(RootPath, "DriverNoYearSelectError.txt");
    private static readonly string HorseNoPanelLog = Path.Combine(RootPath, "HorseNoPanelError.txt");
    private static readonly string NoContentErrorLog = Path.Combine(RootPath, "NoContentError.txt");

    public static async Task AddToDriverNoPanel(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(DriverNoPanelLog, $"{sourceId} :: Driver no panel error\n");
    }

    public static async Task AddToDriverNoYearSelect(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(DriverNoYearSelectLog,  $"{sourceId} :: Driver no year select error\n");
    }

    public static async Task AddToHorseNoPanel(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(HorseNoPanelLog, $"{sourceId} :: Horse no panel error\n");
    }

    public static async Task AddToNoContentError(string url)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(NoContentErrorLog, $"{url} :: NoContent error\n");
    }
}