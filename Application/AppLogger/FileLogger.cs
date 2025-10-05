namespace Application.AppLogger;

public class FileLogger
{
    public static readonly string RootPath = Path.Combine("D:\\Repo\\project_trotting\\", "Logs");
    public static readonly string DriverNoPanelLog = Path.Combine(RootPath, "DriverNoPanelError.txt");

    public static async Task AddToDriverNoPanel(string sourceId)
    {
        Directory.CreateDirectory(RootPath);
        await File.AppendAllTextAsync(DriverNoPanelLog, $"{sourceId} :: Driver no panel added\n");
    }
}