using Microsoft.Playwright;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;

namespace Scraping.Spider.NO;

public class ResultsBotNo(BrowserOptions options, string url) : BaseRobot(options)
{
    // xpaths for elements to havest
    private const string RaceCourseNameXpath = "//article/div/div/div/div[1]";
    private const string RacePanelXpath = "//div[@role=\"tabpanel\"]";
    private const string RacePanelTableRowXpath = "//div[3]/table/tbody/tr[1]";
    
    // url for page navigation
    private string Url { get; } = url;
    
    // list of data collected
    public readonly List<ResultScrapeData> DataCollected = [];

    public async Task Execute(IPage page)
    {
        // navigate to url
        await page.GotoAsync(Url);
        
        // parse competition data
        // var raceCourseElemText = await page.Locator(RaceCourseNameXpath).TextContentAsync()
        // var raceCourse = _parseRaceCourseName(raceCourseElemText!);
        var raceCourse = await ResolveRaceCourseName(page);
        var raceDate = _extractUrlEnd(Url);

        var dataPanel = await page.Locator(RacePanelXpath).AllAsync();


        // extract row data for each data panel
        foreach (var panel in dataPanel)
        {
            var rows = await panel.Locator(RacePanelTableRowXpath).AllAsync();
            foreach (var row in rows)
            {
                var cells = await row.Locator("td").AllAsync();

                var raceNumber = await panel.GetAttributeAsync("id");
                var startNumber = await cells[1].TextContentAsync();
                var driverSourceId = await cells[6].Locator("//a").GetAttributeAsync("href");
                var horseSourceId = await row.Locator("//th//a").GetAttributeAsync("href");
                var distance = await cells[2].TextContentAsync();
                var foreShoe = await cells[7].Locator("//span[1]").GetAttributeAsync("class");
                var hindShoe = await cells[7].Locator("//span[2]").GetAttributeAsync("class");
                var cart = await cells[8].TextContentAsync();
                var odds = await cells[9].TextContentAsync();
                var place = await cells[0].TextContentAsync();
                var time = await cells[3].TextContentAsync();
                var kmTime = await cells[4].TextContentAsync();



                var item = new ResultScrapeData
                {
                    RaceCourse = raceCourse,
                    Date = raceDate,
                    RaceNumber = raceNumber!.Trim(),
                    StartNumber = startNumber!.Trim(),
                    DriverSourceId = _extractUrlEnd(driverSourceId!.Trim()),
                    HorseSourceId = _extractUrlEnd(horseSourceId!.Trim()),
                    Distance = distance!.Trim(),
                    ForeShoe = foreShoe!.Trim(),
                    HindShoe = hindShoe!.Trim(),
                    Cart = cart!.Trim(),
                    Odds = odds!.Trim(),
                    Place = place!.Trim(),
                    Time = time!.Trim(),
                    KmTime = kmTime!.Trim(),
                    FromDirectSource = true
                };

                DataCollected.Add(item);
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
    private string _extractUrlEnd(string url)
    {
        var urlSplit = url.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }
}