using Models.DbModels;

namespace Application.DataServices;

public interface IDataServiceRegistry
{
    IBaseDataService<Competition> CompetitionDataService { get; }
    IDriverDataService DriverDataService { get; }
    IBaseDataService<DriverLicense> DriverLicenseDataService { get; }
    IHorseDataService HorseDataService { get; }
    IBaseDataService<Race> RaceDataService { get; }
    IBaseDataService<Racecourse> RaceCourseDataService { get; }
    IRaceResultDataService RaceResultDataService { get; }
    IRaceStartNumberDataService RaceStartNumberDataService { get; }
    Task InitCaches();
}