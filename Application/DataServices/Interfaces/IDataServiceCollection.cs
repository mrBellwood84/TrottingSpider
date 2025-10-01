using Application.DataServices.Services;
using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IDataServiceCollection
{
    IBaseDataService<Competition> CompetitionDataService { get; }
    IBaseDataService<Driver> DriverDataService { get; }
    IDriverLicenseDataService DriverLicenseDataService { get; }
    IBaseDataService<Horse> HorseDataService { get; }
    IBaseDataService<Race> RaceDataService { get; }
    IBaseDataService<Racecourse> RaceCourseDataService { get; }
    IRaceResultDataService RaceResultDataService { get; }
    IRaceStartNumberDataService RaceStartNumberDataService { get; }
    Task InitCaches();
}