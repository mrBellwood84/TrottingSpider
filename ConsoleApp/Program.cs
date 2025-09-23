using Spider.NO;
using Spider.NO.Data;

string[] idList1 = ["2202", "5404", "30051177"];
string[] idList2 = ["42598", "8111", "2209"];
string[] idList3 = ["119", "29265", "30015553"];

short index = 0;
    
var master = new MasterBotNo();

var driverHarvesterSingle = new DriverHarvestNo(idList1[index]);

await master.RunBrowser(driverHarvesterSingle.Run);

foreach (var item in driverHarvesterSingle.RaceDataCollected)
{
    PrintDriverData(item);
}


void PrintDriverData(ResultScrapeData data)
{
    Console.WriteLine($"\nRaceCourse: {data.RaceCourse}");
    Console.WriteLine($"Date: {data.Date}");
    Console.WriteLine($"RaceNumber of birth: {data.RaceNumber}");
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
