using Application.DataServices.Interfaces;
using Application.DataServices.Services;
using Models.DbModels;

namespace Application.DataServices;

public class DataServiceCollection(
    IBaseDataService<Competition> competitionDataService,
    IBaseDataService<Driver> driverDataService,
    IDriverLicenseDataService driverLicenseDataService,
    IBaseDataService<Horse> horseDataService,
    IBaseDataService<Race> raceDataService,
    IBaseDataService<Racecourse> raceCourseDataService,
    IRaceResultDataService raceResultDataService,
    IRaceStartNumberDataService raceStartNumberDataService) : IDataServiceCollection
{
    public IBaseDataService<Competition> CompetitionDataService { get; } = competitionDataService;
    public IBaseDataService<Driver> DriverDataService { get; } = driverDataService;
    public IDriverLicenseDataService DriverLicenseDataService { get; } = driverLicenseDataService;
    public IBaseDataService<Horse> HorseDataService { get; } = horseDataService;
    public IBaseDataService<Race> RaceDataService { get; } = raceDataService;
    public IBaseDataService<Racecourse> RaceCourseDataService { get; } = raceCourseDataService;
    public IRaceResultDataService RaceResultDataService { get; } = raceResultDataService;
    public IRaceStartNumberDataService RaceStartNumberDataService { get; } = raceStartNumberDataService;

    public async Task InitCaches()
    {
        List<Task> tasks = new List<Task>
        {
            CompetitionDataService.InitCache(),
            DriverDataService.InitCache(),
            DriverLicenseDataService.InitCache(),
            HorseDataService.InitCache(),
            RaceDataService.InitCache(),
            RaceCourseDataService.InitCache(),
            RaceResultDataService.InitCache(),
            RaceStartNumberDataService.InitCache()
        };
        
        await Task.WhenAll(tasks);
        AppLogger.LogNeutral("Data cache initialized.");
    }
}