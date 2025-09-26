using Models.ScrapeData;
using Scraping.Spider.NO.Options;

namespace Scraping.Processors;

public class CalendarDataProcessor
{
    public CalendarLinks Process(CalendarScrapeData data)
    {
        return new CalendarLinks
        {
            Date = _extractUrlEnd(data.StartlistHref),
            RaceCourseName = _parseRaceCourseName(data.CourseAndTime),
            StartlistLink = data.StartlistHref,
            ResultsLink = data.ResultHref
        };
    }

    private string _parseRaceCourseName(string rawData)
    {
        var split = rawData.Split("kl");
        return split[0].Trim().ToUpper();
    }
    
    private string _extractUrlEnd(string url)
    {
        var urlSplit = url.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }
}