using Microsoft.Playwright;
using Spider.NO.Data;

namespace Spider.NO;

public class HorseHarvestNo
{
    // values for min and max option in table select box
    private const short MaxYear = 2025;
    private const short MinYear = 2010;
    
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
    
    // horse source id set in primary constructor
    private string HorseSourceId { get; set; }
    private string[] HorseSourceIds { get; set; }
    
    // collected data
    public List<HorseScrapeData> HorseDataCollected = [];
    public List<ResultScrapeData> RaceDataCollected = [];

    public HorseHarvestNo(string horseSourceId)
    {
        HorseSourceId = horseSourceId;
        HorseSourceIds = [];
    }

    public HorseHarvestNo(string[] horseSourceIds)
    {
        HorseSourceId = string.Empty;
        HorseSourceIds = horseSourceIds;
    }

    public async Task Run(IPage page)
    { 
        if (HorseSourceIds.Length == 0)
        {
            await _runSingle(page, HorseSourceId);
            return;
        }

        foreach (var id in HorseSourceIds)
        {
            Console.WriteLine(id);
            await _runSingle(page, id);
        }
    }
    
    /// <summary>
    /// Run single source id and gater data from sourec
    /// </summary>
    private async Task _runSingle(IPage page, string horseSourceId)
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
            await _harvestResultTable(page, horseSourceId);
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
            SourceId = HorseSourceId,
            Name = name!.Trim(),
            YearOfBirth = yob!.Trim(),
            Sex = sex!.Trim(),
        };
        
        HorseDataCollected.Add(newItem);
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

    private async Task _harvestResultTable(IPage page, string horseSourceId)
    {
        var rows = await page.Locator(ResultTableRowXpath).AllAsync();
        foreach (var row in rows)
        {
            var data = await _parseRow(row, horseSourceId);
            RaceDataCollected.Add(data);
        }
    }

    private async Task<ResultScrapeData> _parseRow(ILocator row, string horseSourceId)
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
          var time = await cells[8].TextContentAsync();
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
              KmTime = time!,
              RRemark = rRemark!,
              GRemark = gRemark!,
              FromDirectSource = false,
          };

          return item;
    }
    
    private string _extractUrlEnd(string url)
    {
        var urlSplit = url.Split('/');
        var length = urlSplit.Length;
        return urlSplit[length - 1].Trim();
    }
    
    

}