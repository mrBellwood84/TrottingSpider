using Application.DataServices;
using Application.DataServices.Services;
using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;

namespace ConsoleApp.Extensions;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // add individual services
        services.AddScoped<IBufferDataService, BufferDataService>();
        services.AddScoped<ICompetitionDataService, CompetitionDataService>();
        services.AddScoped<IDriverDataService, DriverDataService>();
        services.AddScoped<IBaseDataService<DriverLicense>, DriverLicenseDataService>();
        services.AddScoped<IHorseDataService, HorseDataService>();
        services.AddScoped<IBaseDataService<Race>, RaceDataService>();
        services.AddScoped<IBaseDataService<Racecourse>, RacecourseDataService>();
        services.AddScoped<IRaceResultDataService, RaceResultDataService>();
        services.AddScoped<IRaceStartNumberDataService, RaceStartNumberDataService>();
        
        // add collection
        services.AddScoped<IDataServiceRegistry, DataServiceRegistry>();
        
        return services;
    }
}