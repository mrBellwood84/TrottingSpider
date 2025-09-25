using Microsoft.Playwright;
using Models.ScrapeData;
using Models.Settings;

namespace Scraping.Spider.NO;

/// <summary>
/// Harvest links for startlists and for the year and month provided!
/// </summary>
public class CalendarCollectorBot(BrowserOptions options, string year, string month)
    : BaseRobot(options)
{
    private const string CalendarUrl = "https://www.travsport.no/sportsbasen/lopskalender/";

    // Select boxes xpath
    private const string YearSelectXpath = "//select[@id=\"year\"]";
    private const string MonthSelectXpath = "//select[@id=\"month\"]";
    private const string RaceCourseSelectXpath = "//select[@id=\"trackid\"]";

    // calendar items xpath
    private const string RaceCalendarItemsXpath = "//div[@class=\"race-calendar\"]/div";
    private const string CourseAndTimeTextXpath = "//div[@class=\"rc-item__info\"]";
    private const string StartlistLinkXpath = "//a[contains(text(), \"Startliste\")]";
    private const string ResultLinkXpath = "//a[contains(text(), \"Resultater\")]";

    // select box options
    private readonly List<string> _raceCourseOptions  = [];
    
    // options for collector
    private readonly string _year = year;
    private readonly string _month = month;
    
    // collected data to be parsed
    public List<CalendarScrapeData> DataCollected { get; } = [];

    /// <summary>
    /// Run method for iterating race courses at given year and month.
    /// Will collect scraped data in stored unparsed in the DataCollected property. 
    /// </summary>
    /// <param name="page"></param>
    public async Task Run(IPage page)
    {
        await page.GotoAsync(CalendarUrl);

        await page.WaitForSelectorAsync(YearSelectXpath);
        await page.Locator(YearSelectXpath).SelectOptionAsync(_year);

        await page.WaitForSelectorAsync(MonthSelectXpath);
        await page.Locator(MonthSelectXpath).SelectOptionAsync(_month);

        if (_raceCourseOptions.Count == 0) await _initRaceCourseOptions(page);

        foreach (var option in _raceCourseOptions)
        {
            await page.WaitForSelectorAsync(RaceCourseSelectXpath);
            await page.Locator(RaceCourseSelectXpath).SelectOptionAsync(option);
            await _scrapeCalendarData(page);
        }
    }

    /// <summary>
    /// Get race course options in select box. Used if option list is empty
    /// </summary>
    /// <param name="page"></param>
    private async Task _initRaceCourseOptions(IPage page)
    {
        await page.WaitForSelectorAsync(RaceCourseSelectXpath);
        var optionElements = await page.Locator(RaceCourseSelectXpath)
            .Locator("option")
            .AllAsync();

        foreach (var optionElement in optionElements)
        {
            var value = await optionElement.GetAttributeAsync("value");
            if (value == "0") continue;
            _raceCourseOptions.Add(value!);
        }
    }

    private async Task _scrapeCalendarData(IPage page)
    {
        var elements = await page.Locator(RaceCalendarItemsXpath).AllAsync();

        var dateList = new List<ILocator>();
        var dataList = new List<ILocator>();

        foreach (var elem in elements)
        {
            var className = await elem.GetAttributeAsync("class");
            switch (className)
            {
                case "rc-date":
                    dateList.Add(elem);
                    break;
                case "rc-item":
                    dataList.Add(elem);
                    break;
            }
        }

        if (dateList.Count != dataList.Count)
            throw new Exception(
                $"Error occured when parsing race calendar data. Date count was {dateList.Count} and data count was {dataList.Count}");
        
        for (var i = 0; i < dateList.Count; i++)
        {
            var item = new CalendarScrapeData();
            
            var date = await dateList[i].TextContentAsync();
            var trackAndTime = await dataList[i].Locator(CourseAndTimeTextXpath).TextContentAsync();
            string? startlistLink = null;
            string? resultLink = null;

            try
            {
                startlistLink = await dataList[i].Locator(StartlistLinkXpath)
                    .GetAttributeAsync("href", new() { Timeout = 200 });
            }
            catch (TimeoutException) {}

            try
            {
               resultLink = await dataList[i].Locator(ResultLinkXpath)
                   .GetAttributeAsync("href", new () { Timeout = 200 }); 
            }
            catch (TimeoutException) {}

            item.Date = date != null ? date.Trim() : string.Empty;
            item.CourseAndTime = trackAndTime != null ? trackAndTime.Trim() : string.Empty;
            item.StartlistHref = startlistLink ??  string.Empty;
            item.ResultHref = resultLink ??  string.Empty;
            
            if (item.StartlistHref == string.Empty) continue;
            DataCollected.Add(item);
        }
    }
}