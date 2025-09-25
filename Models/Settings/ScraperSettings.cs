namespace Models.Settings;

public class ScraperSettings
{
    public string MaxYear { get; init; }
    public string MinYear { get; init; }
    public string[] MonthRange { get; init; }
    public string[] YearRange { get; init; } 
}