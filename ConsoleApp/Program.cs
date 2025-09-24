using Spider.NO;
using Spider.NO.Data;

string url_1 = "https://www.travsport.no/travbaner/biri-travbane/results/2025-09-19";
    
var master = new MasterBotNo();

var harvester = new ResultsHarvestNo(url_1);

await master.RunBrowser(harvester.Run);

foreach (var data in harvester.DataCollected[0..1])
{
    PrintDriverData(data);
}

void PrintDriverData(ResultScrapeData data)
{
    Console.WriteLine($"\nRaceCourse: {data.RaceCourse}");
    Console.WriteLine($"Date: {data.Date}");
    Console.WriteLine($"RaceNumber: {data.RaceNumber}");
    Console.WriteLine($"StartNumber: {data.StartNumber}");
    Console.WriteLine($"DriverSource: {data.DriverSourceId}");
    Console.WriteLine($"HorseSource: {data.HorseSourceId}");
    Console.WriteLine($"TrackNumber: {data.TrackNumber}");
    Console.WriteLine($"Distance: {data.Distance}");
    Console.WriteLine($"ForeShoe: {data.ForeShoe}");
    Console.WriteLine($"HindShoe: {data.HindShoe}");
    Console.WriteLine($"Cart: {data.Cart}");
    Console.WriteLine($"Place: {data.Place}");
    Console.WriteLine($"Time: {data.Time}");
    Console.WriteLine($"KmTime: {data.KmTime}");
    Console.WriteLine($"RRemark: {data.RRemark}");
    Console.WriteLine($"GRemark: {data.GRemark}");
    Console.WriteLine($"FromDirectSource: {data.FromDirectSource}");
}