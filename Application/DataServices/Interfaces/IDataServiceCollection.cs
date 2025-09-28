using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IDataServiceCollection
{
    IBaseDataService<Competition> CompetitionDataService { get; }
    IBaseDataService<Driver> DriverDataService { get; }
    IBaseDataService<DriverLicense> DriverLicenseDataService { get; }
    IBaseDataService<Horse> HorseDataService { get; }
    IBaseDataService<Race> RaceDataService { get; }
    IBaseDataService<Racecourse> RaceCourseDataService { get; }
    IBaseDataService<RaceResult> RaceResultDataService { get; }
    IBaseDataService<RaceStartNumber> RaceStartNumberDataService { get; }
    Task InitCaches();
}