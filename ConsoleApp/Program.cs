using Application.AppLogger;
using ConsoleApp;
using Microsoft.Playwright;
using Models.Record;
using Models.ScrapeData;
using Persistence;
using Scraping.Processor;
using Scraping.Spider;
using Scraping.Spider.NO;

Console.Clear();

string startlist = "https://www.travsport.no/travbaner/bjerke-travbane/startlist/2025-09-23";
string resultlist = "https://www.travsport.no/travbaner/bjerke-travbane/results/2025-09-23";
string[] driverSourceIds = ["5550", "42598", "69352"];
string[] horseSourceIds = ["578001020220130", "578001020220158", "578001020220392"];

var config = new AppConfigurations();
var logger = new AppLogger();

var competitionDbService = new CompetitionDbService(config.ConnectionString!);
var driverDbService = new DriverDbService(config.ConnectionString!);
var licenceDbService = new DriverLicenseBaseDbService(config.ConnectionString!);
var horseDbService = new HorseDbService(config.ConnectionString!);
var raceDbService = new RaceDbService(config.ConnectionString!);
var raceCourseDbService = new RaceCourseDbService(config.ConnectionString!);
var raceStartNumberDbService = new RaceStartNumberDbService(config.ConnectionString!);
var raceResultDbService = new RaceResultDbService(config.ConnectionString!);

var competitionIds = await competitionDbService.GetIdDictionary();
var driverIds = await driverDbService.GetIdDictionary();
var licenseIds = await licenceDbService.GetIdDictionary();
var horseIds = await horseDbService.GetIdDictionary();
var raceIds = await raceDbService.GetIdDictionary();
var raceCourseIds = await raceCourseDbService.GetIdDictionary();
var raceStartNumberIds = await raceStartNumberDbService.GetIdDictionary();
var raceResultIds = await raceResultDbService.GetIdDictionary();

var bot = new BaseRobot();


var selectedDriver = driverSourceIds[0];
var selectedHorse =  horseSourceIds[0];

var driverBot = new DriverBotNo(selectedDriver);
var horseBot = new HorseBotNo(selectedHorse);

if (!driverIds.ContainsKey(selectedDriver))
{
    await bot.RunBrowser(driverBot.Run);
    var driverParser =  new ProcessDriverScrapeData(licenseIds);
    var result = driverParser.Process(driverBot.DriverDataCollected[0]);
    if (driverParser.CreateNewDriverLicense != null)
    {
        await licenceDbService.CreateAsync(driverParser.CreateNewDriverLicense);
        driverParser.CreateNewDriverLicense = null;
    }
    await driverDbService.CreateAsync(result);
    Console.WriteLine($"Driver id: {result.Id}");
    Console.WriteLine($"Driver SourceId: {result.SourceId}");
    Console.WriteLine($"Driver name: {result.Name}");
    Console.WriteLine($"Driver Year Of Birth: {result.YearOfBirth}");
    Console.WriteLine($"Driver Licence: {result.DriverLicenseId}");
}

if (!horseIds.ContainsKey(selectedHorse))
{
    await bot.RunBrowser(horseBot.Run);
    var horseParser = new ProcessHorseScrapeData();
    var result = horseParser.Process(horseBot.HorseDataCollected[0]);
    await horseDbService.CreateAsync(result);
    
    Console.WriteLine($"Horse id: {result.Id}");
    Console.WriteLine($"Horse SourceId: {result.SourceId}");
    Console.WriteLine($"Horse name: {result.Name}");
    Console.WriteLine($"Horse sex: {result.Sex}");
    Console.WriteLine($"Horse Year Of Birth: {result.YearOfBirth}");
}

