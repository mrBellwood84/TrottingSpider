using Microsoft.Playwright;
using Models.ScrapeData;

namespace Scraping.Spider.NO;

/// <summary>
/// Harvest Data from drivers page. Require source id of driver to run.
/// Does also accept a range of driver source ids.
/// </summary>
public class DriverHarvestNo
{
    // values for min and max option in table select box
    private const short MaxYear = 2010;
    private const short MinYear = 2010;
    
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
    
    // driver id set in primary constructors
    private string DriverSourceId { get; set; }
    private string[] DriverSourceIds { get; set; }

    /// <summary>
    /// List of scraped driver data
    /// </summary>
    public List<DriverScapeData> DriverDataCollected = [];
    /// <summary>
    /// List of scraped result table data
    /// </summary>
    public List<ResultScrapeData> RaceDataCollected = [];

    // constructors
    public DriverHarvestNo(string driverSourceId)
    {
        DriverSourceId = driverSourceId;
        DriverSourceIds = [];
    }

    public DriverHarvestNo(string[] horseSourceIds)
    {
        DriverSourceId = string.Empty;
        DriverSourceIds = horseSourceIds;
    }

    /// <summary>
    /// Main method for running driver harvester. Will run dependent of range of provided id's
    /// </summary>
    public async Task Run(IPage page)
    {
        if (DriverSourceIds.Length == 0)
        {
            await _runSingle(page, DriverSourceId);
            return;
        }

        foreach (var item in DriverSourceIds)
        {
            await _runSingle(page, item);
        }
    }
    
    /// <summary>
    /// Get data from page at provided driver id
    /// </summary>
    private async Task _runSingle(IPage page, string driverSourceId)
    {
        // create url and navigate
        var url = $"{BaseUrl}{driverSourceId}"; 
        await page.GotoAsync(url);
        
        // havest driver data
        await _harvestDriverData(page, driverSourceId);
        
        // resolve year select options
        // await page.Locator(StartsButtonXpath).ClickAsync();
        
        var yearOptions = await _resolveYearOptions(page);
        foreach (var year in yearOptions)
        {
            await page.WaitForSelectorAsync(YearSelectXpath);
            await page.Locator(YearSelectXpath).SelectOptionAsync(year);
            await page.WaitForSelectorAsync(RowSelectXpath);
            await page.Locator(RowSelectXpath).SelectOptionAsync("Alle");
            await _harvestResultTable(page, driverSourceId);
        }
    }
    
    /// <summary>
    /// Harvest driver data from page
    /// </summary>
    private async Task _harvestDriverData(IPage page, string driverSourceId)
    {
        var listItems = await page.Locator(DriverInfoListXpath).AllAsync();

        var name = string.Empty;
        var yob = await page.Locator(YearOfBirthXpath).TextContentAsync();
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

        var newItem = new DriverScapeData
        {
            SourceId = driverSourceId,
            Name = name!.Trim(),
            YearOfBirth = yob!.Trim(),
            DriverLicense = driverLicense!.Trim(),
        };
        DriverDataCollected.Add(newItem);
    }
    
    /// <summary>
    /// Get rows in table and iterate for parsing 
    /// </summary>
    private async Task _harvestResultTable(IPage page, string driverSourceId)
    {
        var rows =  await page.Locator(ResultTableRowsXpath).AllAsync();
        foreach (var row in rows)
        {
            var data = await _parseRow(row, driverSourceId);
            RaceDataCollected.Add(data);
        }
    }

    private async Task<ResultScrapeData> _parseRow(ILocator row, string driverSourceId)
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
        await page.Locator(StartsButtonXpath).ClickAsync();
        await page.WaitForSelectorAsync(YearSelectXpath);
        var optionElements  = await page.Locator(YearSelectXpath).Locator("option").AllAsync();
        
        foreach (var elem in optionElements)
        {
            var value = await elem.GetAttributeAsync("value");
            
            if (value == "") continue;
            if (int.Parse(value!) > MaxYear) continue;
            if (int.Parse(value!) < MinYear) continue;
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
    
}