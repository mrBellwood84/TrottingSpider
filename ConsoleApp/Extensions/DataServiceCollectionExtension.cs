using Application.CacheServices;
using Application.DataServices;
using Application.DataServices.Interfaces;
using Application.DataServices.Services;
using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;

namespace ConsoleApp.Extensions;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // add individual services
        services.AddScoped<IBaseDataService<Competition>, CompetitionDataService>();
        services.AddScoped<IDriverDataService, DriverDataService>();
        services.AddScoped<IDriverLicenseDataService , DriverLicenseDataService>();
        services.AddScoped<IHorseDataService, HorseDataService>();
        services.AddScoped<IBaseDataService<Race>, RaceDataService>();
        services.AddScoped<IBaseDataService<Racecourse>, RacecourseDataService>();
        services.AddScoped<IRaceResultDataService, RaceResultDataService>();
        services.AddScoped<IRaceStartNumberDataService, RaceStartNumberDataService>();
        
        // add collection
        services.AddScoped<IDataServiceCollection, DataServiceCollection>();
        
        return services;
    }
}