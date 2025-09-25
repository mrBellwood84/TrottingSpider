using Models.ScrapeData;
using Scraping.Spider.NO.Options;

namespace Scraping.Processors;

public class CalendarDataProcessor
{
    public CalendarLinks Process(CalendarScrapeData data)
    {
        return new CalendarLinks
        {
            Date = _extractUrlEnd(data.Date),
            RaceCourseName = _parseRaceCourseName(data.CourseAndTime),
            StartlistLink = data.StartlistHref,
            ResultsLink = data.ResultHref
        };
    }

    private string _parseRaceCourseName(string rawData)
    {
        var split = rawData.Split(" ");
        var l = split.Length - 2;
        return string.Join(" ", split[..l]).ToUpper();
    }
    
    private string _extractUrlEnd(string url)
    {
        var urlSplit = url.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }
}