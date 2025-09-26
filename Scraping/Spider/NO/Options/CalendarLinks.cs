namespace Scraping.Spider.NO.Options;

public struct CalendarLinks
{
    public CalendarLinks() { }

    public string Date { get; init; } = null;
    public string RaceCourseName { get; init; } = null;
    public string StartlistLink { get; init; } = null;
    public bool StartlistFromSource { get; set; } = false;
    public string ResultsLink { get; init; } = null;
    public bool ResultsFromSource { get; set; } = false;
    
}