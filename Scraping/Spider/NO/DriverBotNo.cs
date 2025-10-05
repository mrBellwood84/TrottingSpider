using Microsoft.Playwright;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;

namespace Scraping.Spider.NO;

/// <summary>
/// Harvest Data from drivers page. Require source id of driver to run.
/// Does also accept a range of driver source ids.
/// </summary>
public class DriverBotNo(
    BrowserOptions browserOptions, 
    ScraperSettings scraperSettings, 
    string driverSourceId) : BaseRobot(browserOptions)
{
    // driver data xpaths
    private const string YearOfBirthXpath = "//article//h1/small";
    private const string DriverInfoListXpath = "//ul[@class=\"details-row\"]/li";
    
    // action element xpaths
    private const string StartsButtonXpath = "//a[@aria-controls=\"raceList\"]";
    private const string YearSelectXpath = "//select[@id=\"startYear\"]";
    private const string RowSelectXpath = "//section[@id=\"raceList\"]//select[not(@name=\"startYear\")]";
    
    // result table rows xpath
    private const string ResultTableRowsXpath = "//section[@id=\"raceList\"]//tbody/tr";
    
    //base url for driver data
    private const string BaseUrl = "https://www.travsport.no/sportsbasen/sportssok/driver/";
    
    /// <summary>
    /// List of scraped driver data
    /// </summary>
    public DriverScrapeData DriverDataCollected { get; private set; }
    /// <summary>
    /// List of scraped result table data
    /// </summary>
    public readonly List<ResultScrapeData> RaceDataCollected = [];

    
    /// <summary>
    /// Get data from page at provided driver id
    /// </summary>
    public async Task Execute(IPage page)
    {
        // create url and navigate
        var url = $"{BaseUrl}{driverSourceId}"; 
        await page.GotoAsync(url);
        
        // havest driver data
        await _harvestDriverData(page);
        
        // resolve year select options
        // await page.Locator(StartsButtonXpath).ClickAsync();
        
        var yearOptions = await _resolveYearOptions(page);
        foreach (var year in yearOptions)
        {
            await page.WaitForSelectorAsync(YearSelectXpath);
            await page.Locator(YearSelectXpath).SelectOptionAsync(year);
            await page.WaitForSelectorAsync(RowSelectXpath);
            await page.Locator(RowSelectXpath).SelectOptionAsync("Alle");
            await _harvestResultTable(page);
        }
    }
    
    /// <summary>
    /// Harvest driver data from page
    /// </summary>
    private async Task _harvestDriverData(IPage page)
    {
        var listItems = await page.Locator(DriverInfoListXpath).AllAsync();

        var name = string.Empty;
        var yob = await ResolveYearOfBirth(page);
        var driverLicense = string.Empty;

        foreach (var item in listItems)
        {
            var spans = await item.Locator("span").AllAsync();
            var label = await spans[0].TextContentAsync();
            var content = await spans[1].TextContentAsync();

            switch (label)
            {
                case "Navn":
                    name = content;
                    break;
                case "Lisens":
                    driverLicense = content;
                    break;
            }
        }

        var newItem = new DriverScrapeData
        {
            SourceId = driverSourceId,
            Name = name!.Trim(),
            YearOfBirth = yob!.Trim(),
            DriverLicense = driverLicense!.Trim(),
        };
        DriverDataCollected = newItem;
    }
    
    /// <summary>
    /// Get rows in table and iterate for parsing 
    /// </summary>
    private async Task _harvestResultTable(IPage page)
    {
        var rows =  await page.Locator(ResultTableRowsXpath).AllAsync();
        foreach (var row in rows)
        {
            var data = await _parseRow(row);
            RaceDataCollected.Add(data);
        }
    }

    private async Task<ResultScrapeData> _parseRow(ILocator row)
    {
        var cells = await row.Locator("td").AllAsync();
        
        var raceCourse = await cells[2].TextContentAsync();
        var date = await cells[4].TextContentAsync();
        var raceNumber = await cells[5].TextContentAsync();
        var startNumber = await cells[17].TextContentAsync();
        var horseSourceId = await cells[1].Locator("a").GetAttributeAsync("href");
        var trackDist = await cells[7].TextContentAsync();
        var foreShoe = await cells[18].Locator("//span[1]").GetAttributeAsync("class");
        var hindShoe = await cells[18].Locator("//span[2]").GetAttributeAsync("class");
        var cart = await cells[19].TextContentAsync();
        var place = await cells[9].TextContentAsync();
        var kmTime = await cells[11].TextContentAsync();
        var odds = await cells[14].TextContentAsync();
        var rRemark =  await cells[12].TextContentAsync();
        var gRemark = await cells[13].TextContentAsync();

        var item = new ResultScrapeData
        {
            RaceCourse = raceCourse!,
            Date = date!,
            RaceNumber = raceNumber!,
            StartNumber = startNumber!,
            DriverSourceId = driverSourceId,
            HorseSourceId = _extractUrlEnd(horseSourceId!),
            TrackNumber = trackDist!.Split("/")[0],
            Distance = trackDist!.Split("/")[1],
            ForeShoe = foreShoe!,
            HindShoe = hindShoe!,
            Cart = cart!,
            Place = place!,
            KmTime = kmTime!,
            Odds = odds!,
            RRemark = rRemark!,
            GRemark = gRemark!,
            FromDirectSource = false,
        };
        return item;
    }
    
    /// <summary>
    /// Get year options from select box within the min/max limit
    /// </summary>
    private async Task<List<string>> _resolveYearOptions(IPage page)
    {
        var result = new List<string>();
        await ClickStartPanelButton(page);
        // await page.Locator(StartsButtonXpath).ClickAsync();
        await page.WaitForSelectorAsync(YearSelectXpath);
        var optionElements  = await page.Locator(YearSelectXpath).Locator("option").AllAsync();
        
        foreach (var elem in optionElements)
        {
            var value = await elem.GetAttributeAsync("value");
            
            if (value == "") continue;
            if (int.Parse(value!) > int.Parse(scraperSettings.MaxYear)) continue;
            if (int.Parse(value!) < int.Parse(scraperSettings.MinYear)) continue;
            result.Add(value!);
        }
        return result;
    }
    
    /// <summary>
    /// Get last part of url string
    /// </summary>
    private string _extractUrlEnd(string url)
    {
        var urlSplit = url.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }

    private async Task ClickStartPanelButton(IPage page)
    {
        try
        {
            await page.Locator(StartsButtonXpath).ClickAsync();
        }
        catch (TimeoutException ex)
        {
            throw new NoPanelButtonException("Panel button not found!", ex);
        }
    }
    
    /// <summary>
    ///  Resolve year of birth, return 1900 if no year can be provided
    /// </summary>
    private async Task<string> ResolveYearOfBirth(IPage page)
    {
        try
        {
            var result = await page.Locator(YearOfBirthXpath).TextContentAsync();
            return result;
        }
        catch (TimeoutException)
        {
            return "1900";
        }
    }
}