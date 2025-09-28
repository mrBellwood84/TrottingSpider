using Application.CacheServices;
using Application.DataServices;
using Application.DataServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;

namespace ConsoleApp.Extensions;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // add individual services

        services.AddScoped<IBaseDataService<Competition>, CompetitionDataService>();
        services.AddScoped<IBaseDataService<Driver>, DriverDataService>();
        services.AddScoped<IBaseDataService<DriverLicense> , DriverLicenseDataService>();
        services.AddScoped<IBaseDataService<Horse>, HorseDataService>();
        services.AddScoped<IBaseDataService<Race>, RaceDataService>();
        services.AddScoped<IBaseDataService<Racecourse>, RacecourseDataService>();
        services.AddScoped<IBaseDataService<RaceResult>, RaceResultDataService>();
        services.AddScoped<IBaseDataService<RaceStartNumber>, RaceStartNumberDataService>();
        
        // add collection
        services.AddScoped<IDataServiceCollection, DataServiceCollection>();
        
        return services;
    }
}