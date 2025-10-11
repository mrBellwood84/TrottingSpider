using Microsoft.Playwright;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;

namespace Scraping.Spider.NO;

/// <summary>
/// Harvest data from startlist pages. 
/// </summary>
/// <param name="url"></param>
public class StartlistBotNo(BrowserOptions options, string url) : BaseRobot(options)
{
    // xpaths for elements to harvest
    private const string RaceCourseNameXpath = "//article/div/div/div/div[1]";
    private const string RaceNavXpath = "//div[@class=\"race-navigation\"]";
    private const string RacePanelXpath = "//div[@class=\"js-tabbedContent-panel\"]";
    
    // xpaths inside Race panel
    private const string RacePanelStrongXpath = "//p/strong";
    private const string RaceTableRowsXpath = "//div[2]/table/tbody/tr[not(@class=\"expandable-row\")]";
    
    /// <summary>
    /// Url for page navigation
    /// </summary>
    private string Url { get; } = url;
    
    /// <summary>
    /// List of data collected
    /// </summary>
    public List<StartlistScrapeData> CollectedData { get; } = [];

    public async Task Execute(IPage page)
    {
        await page.GotoAsync(Url);
        
        // parse Competition data here
        // var raceCourseElemText = await page.Locator(RaceCourseNameXpath).TextContentAsync();
        //var raceCourseName = _parseRaceCourseName(raceCourseElemText!);
        var raceCourseName = await ResolveRaceCourseName(page);
        var raceDate = _extractUrlEnd(Url);
        
        // extract race numbers from element
        var raceNumbers = new List<string>();
        foreach (var elem in await page.Locator(RaceNavXpath).AllAsync())
        {
            var number = await elem.GetAttributeAsync("id");
            raceNumbers.Add(number!);
        }
        
        // get data panels
        var dataPanels = await page.Locator(RacePanelXpath).AllAsync();

        for (var i = 0; i < raceNumbers.Count; i++)
        {
            // get race number and check if race is part of gambling!
            var raceNumber = raceNumbers[i];
            var gamblingCategories = await dataPanels[i].Locator(RacePanelStrongXpath).AllAsync();
            var hasGambling = gamblingCategories.Count > 0;
                
            // parse each row, extract cell info
            foreach (var row in await dataPanels[i].Locator(RaceTableRowsXpath).AllAsync())
            {
                var cells = await row.Locator("td").AllAsync();
                
                var startNumber = await cells[0].TextContentAsync();
                var horseLink = await row.Locator("//th/a").GetAttributeAsync("href");
                var driverLink = await cells[1].Locator("a").GetAttributeAsync("href");
                var trackNumber = await cells[2].TextContentAsync();
                var foreShoe = await cells[3].Locator("//span[1]").GetAttributeAsync("class");
                var hindShoe = await cells[3].Locator("//span[2]").GetAttributeAsync("class");
                var turn = await cells[7].TextContentAsync();
                var auto = await cells[8].TextContentAsync();
                var distance = await cells[9].TextContentAsync();
                var cart = await cells[10].TextContentAsync();
                
                var item = new StartlistScrapeData
                {
                    RaceCourse = raceCourseName,
                    Date = raceDate,
                    RaceNumber = raceNumber,
                    StartNumber = startNumber!.Trim(),
                    HorseSourceId = _extractUrlEnd(horseLink),
                    DriverSourceId = _extractUrlEnd(driverLink),
                    ForeShoe = foreShoe!,
                    HindShoe = hindShoe!,
                    TrackNumber = trackNumber!.Trim(),
                    Turn = turn!.Trim(),
                    Auto = auto!.Trim(),
                    Distance = distance!.Trim(),
                    Cart = cart!.Trim(),
                    HasGambling = hasGambling
                };
                CollectedData.Add(item);
            }
        }
    }

    private async Task<string> ResolveRaceCourseName(IPage page)
    {
        try
        {
            var raceCourseElemTexT = await page.Locator(RaceCourseNameXpath)
                .TextContentAsync(new LocatorTextContentOptions() { Timeout = 4000 });
            return _parseRaceCourseName(raceCourseElemTexT!);
        }
        catch (Exception ex)
        {
            throw new NoContentException("No Race startlist was found in page!", ex);
        }
    }

    
    /// <summary>
    /// Parse Racecourse name for element text
    /// </summary>
    private string _parseRaceCourseName(string elementText)
    {
        var spaceSplit = elementText.Split(' ');
        var length = spaceSplit.Length - 4;
        var subArray = spaceSplit[..length];
        return string.Join(" ", subArray).Trim();
    }
    
    /// <summary>
    /// Parse date from url string
    /// </summary>
    private string _extractUrlEnd(string link)
    {
        var urlSplit = link.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }
}