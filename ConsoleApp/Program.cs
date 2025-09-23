using Spider.NO;

var recentGame = "https://www.travsport.no/travbaner/jarlsberg-travbane/startlist/2025-09-21";
var oldGame = "https://www.travsport.no/travbaner/bjerke-travbane/startlist/2015-09-30";

var master = new MasterBotNo();

var testGame = new StartlistHarvestNo(oldGame);

await master.RunBrowser(testGame.Run);


foreach (var item in testGame.CollectedData[5..8])
{
    Console.WriteLine($"RaceCourse: {item.RaceCourse}");
    Console.WriteLine($"Date: {item.Date}");
    Console.WriteLine($"RaceNumber: {item.RaceNumber}");
    Console.WriteLine($"StartNumber: {item.StartNumber}");
    Console.WriteLine($"HorseSourceId: {item.HorseSourceId}");
    Console.WriteLine($"DriverSourceId: {item.DriverSourceId}");
    Console.WriteLine($"TrackNumber: {item.TrackNumber}");
    Console.WriteLine($"ForeShoe {item.ForeShoe}");
    Console.WriteLine($"HindShoe {item.HindShoe}");
    Console.WriteLine($"Turn {item.Turn}");
    Console.WriteLine($"Aut: {item.Auto}");
    Console.WriteLine($"Distance: {item.Distance}");
    Console.WriteLine($"Cart: {item.Cart}");
    Console.WriteLine($"HasGambling: {item.HasGambling}\n");
}

