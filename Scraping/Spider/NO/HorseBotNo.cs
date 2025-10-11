using Microsoft.Playwright;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;

namespace Scraping.Spider.NO;

/// <summary>
/// Harvests data from horse page. Require horse source id to run
/// Does allow a range of drivers source ids
/// </summary>
public class HorseBotNo(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    string horseSourceId) : BaseRobot (browserOptions)
{
    //base url for horse data
    private const string BaseUrl = "https://www.travsport.no/sportsbasen/sportssok/horse/";
    
    // horse data xpaths
    private const string NameElementXpath = "//main//h1";
    private const string HorseInfoListXpath = "//ul[@class=\"details-row\"]/li";
    
    // action element xpaths
    private const string StartsButtonXpath = "//a[@aria-controls=\"starts\"]";
    private const string YearSelectXpath = "//select[@id=\"startYear\"]";
    private const string RowSelectXpath = "//section[@id=\"starts\"]//select[@class=\"dataTable-selector\"]";
    
    // result table xpaths
    private const string ResultTableRowXpath = "//section[@id=\"starts\"]//tbody/tr";
    

    /// <summary>
    /// List of scraped horse data
    /// </summary>
    public HorseScrapeData HorseDataCollected { get; private set; }
    /// <summary>
    /// List of scraped result table data
    /// </summary>
    public List<ResultScrapeData> RaceDataCollected { get; } = [];


    /// <summary>
    /// Run single source id and gater data from source
    /// </summary>
    public async Task Execute(IPage page)
    {
        // create url and navigate
        var url = $"{BaseUrl}{horseSourceId}";
        await page.GotoAsync(url);
        
        // harvest horse data
        await _harvestHorseData(page);
        
        // resolve year select options
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
    /// harvest horse data from page
    /// </summary>
    private async Task _harvestHorseData(IPage page)
    {
        var nameRaw = await page.Locator(NameElementXpath).TextContentAsync();
        var listItems = await page.Locator(HorseInfoListXpath).AllAsync();

        var name = nameRaw!.Split("(")[0];
        var yob = string.Empty;
        var sex = string.Empty;

        foreach (var item in listItems)
        {
            var spans = await item.Locator("span").AllAsync();
            
            var label = await spans[0].TextContentAsync();
            var content = await spans[2].TextContentAsync();
            
            switch (label)
            {
                case "Årgang:":
                    yob = content;
                    break;
                case "Kjønn:":
                    sex  = content;
                    break;
            }
        }

        var newItem = new HorseScrapeData
        {
            SourceId = horseSourceId,
            Name = name!.Trim(),
            YearOfBirth = yob!.Trim(),
            Sex = sex!.Trim()
        };
        
        HorseDataCollected = newItem;
    }
    
    /// <summary>
    /// Get data from result table at horse page
    /// </summary>
    private async Task _harvestResultTable(IPage page)
    {
        var rows = await page.Locator(ResultTableRowXpath).AllAsync();
        foreach (var row in rows)
        {
            var data = await _parseRow(row);
            RaceDataCollected.Add(data);
        }
    }

    /// <summary>
    /// Parse each row for result in result table in horse page
    /// </summary>
    private async Task<ResultScrapeData> _parseRow(ILocator row)
    {
        var cells = await row.Locator("td").AllAsync();

        var raceCourse = await cells[1].TextContentAsync();
        var date = await cells[2].TextContentAsync();
        var raceNumber = await cells[3].TextContentAsync();
        var startNumber = await cells[6].TextContentAsync();
        var driverSourceId = await cells[0].Locator("a").GetAttributeAsync("href");
        var trackAndDistance = await cells[4].TextContentAsync();
        var foreShoe = await cells[15].Locator("//span[1]").GetAttributeAsync("class");
        var hindShoe = await cells[15].Locator("//span[2]").GetAttributeAsync("class");
        var cart = await cells[16].TextContentAsync();
        var place = await cells[9].TextContentAsync(); 
        var kmTime = await cells[8].TextContentAsync();
        var odds = await cells[12].TextContentAsync();
        var rRemark = await cells[10].TextContentAsync();
        var gRemark = await cells[11].TextContentAsync();

        var item = new ResultScrapeData
        {
            RaceCourse = raceCourse!,
            Date = date!,
            RaceNumber = raceNumber!,
            StartNumber = startNumber!,
            DriverSourceId = _extractUrlEnd(driverSourceId!),
            HorseSourceId = horseSourceId,
            TrackNumber = trackAndDistance!.Split("/")[0],
            Distance = trackAndDistance!.Split("/")[1],
            ForeShoe = foreShoe!,
            HindShoe = hindShoe!,
            Cart = cart!,
            Place = place!,
            KmTime = kmTime!,
            Odds = odds!,
            RRemark = rRemark!,
            GRemark = gRemark!,
            FromDirectSource = false
        };
        return item;
    }
    
    /// <summary>
        /// Get year options from select box within the min/max limit
        /// </summary>
    private async Task<List<string>> _resolveYearOptions(IPage page)
    {
        var result = new List<string>();
        // await page.Locator(StartsButtonXpath).ClickAsync();
        await ClickStartPanelButton(page);
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
    
    private async Task ClickStartPanelButton(IPage page)
    {
        try
        {
            await page.Locator(StartsButtonXpath).ClickAsync(new LocatorClickOptions {Timeout = 4000});
        }
        catch (TimeoutException ex)
        {
            throw new NoPanelButtonException("Panel button not found!", ex);
        }
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
}