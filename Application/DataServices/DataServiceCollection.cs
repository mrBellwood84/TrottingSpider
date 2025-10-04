using Application.DataServices.Interfaces;
using Application.DataServices.Services;
using Models.DbModels;

namespace Application.DataServices;

public class DataServiceCollection(
    IBaseDataService<Competition> competitionDataService,
    IDriverDataService driverDataService,
    IBaseDataService<DriverLicense> driverLicenseDataService,
    IHorseDataService horseDataService,
    IBaseDataService<Race> raceDataService,
    IBaseDataService<Racecourse> raceCourseDataService,
    IRaceResultDataService raceResultDataService,
    IRaceStartNumberDataService raceStartNumberDataService) : IDataServiceCollection
{
    public IBaseDataService<Competition> CompetitionDataService { get; } = competitionDataService;
    public IDriverDataService DriverDataService { get; } = driverDataService;
    public IBaseDataService<DriverLicense> DriverLicenseDataService { get; } = driverLicenseDataService;
    public IHorseDataService HorseDataService { get; } = horseDataService;
    public IBaseDataService<Race> RaceDataService { get; } = raceDataService;
    public IBaseDataService<Racecourse> RaceCourseDataService { get; } = raceCourseDataService;
    public IRaceResultDataService RaceResultDataService { get; } = raceResultDataService;
    public IRaceStartNumberDataService RaceStartNumberDataService { get; } = raceStartNumberDataService;

    public async Task InitCaches()
    {
        List<Task> tasks =
        [
            CompetitionDataService.InitCache(),
            DriverDataService.InitDriverCacheAsync(),
            DriverLicenseDataService.InitCache(),
            HorseDataService.InitHorseCacheAsync(),
            RaceDataService.InitCache(),
            RaceCourseDataService.InitCache(),
            RaceResultDataService.InitCache(),
            RaceStartNumberDataService.InitCache()
        ];
        
        await Task.WhenAll(tasks);
        AppLogger.LogNeutral("Data cache initialized.");
    }
}