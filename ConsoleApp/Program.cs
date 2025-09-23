using Spider.NO;

string[] idlist_1 = ["N-07-0686", "DK-0D2107", "578001020185073"];
short index = 2;
    
var master = new MasterBotNo();
var horseHarvester = new HorseHarvestNo(idlist_1[index]);

await master.RunBrowser(horseHarvester.Run);

foreach (var item in horseHarvester.RaceDataCollected[0..3])
{
    Console.WriteLine($"\nRacecourse: {item.RaceCourse}");
    Console.WriteLine($"Date: {item.Date}");
    Console.WriteLine($"Racenumber: {item.RaceNumber}");
    Console.WriteLine($"StartNumber: {item.StartNumber}");
    Console.WriteLine($"DriverSourceId: {item.DriverSourceId}");
    Console.WriteLine($"HorseSourceId: {item.HorseSourceId}");
    Console.WriteLine($"TrackNumber: {item.TrackNumber}");
    Console.WriteLine($"Distance: {item.Distance}");
    Console.WriteLine($"ForeShoe: {item.ForeShoe}");
    Console.WriteLine($"HindShoe: {item.HindShoe}");
    Console.WriteLine($"Cart: {item.Cart}");
    Console.WriteLine($"Place: {item.Place}");
    Console.WriteLine($"KmTime: {item.KmTime}");
    Console.WriteLine($"RRemark: {item.RRemark}");
    Console.WriteLine($"GRemark: {item.GRemark}");
    Console.WriteLine($"FromDirectSource: {item.FromDirectSource}\n");
    
    
    
    
    
}
