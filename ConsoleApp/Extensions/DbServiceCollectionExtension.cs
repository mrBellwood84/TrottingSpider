using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;
using Persistence.DbServices;
using Persistence.Interfaces;

namespace ConsoleApp.Extensions;

public static class DbServiceCollectionExtension
{
    public static IServiceCollection AddDbServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseDbService<Competition>, CompetitionDbService>();
        services.AddScoped<IBaseDbService<Driver>, DriverDbService>();
        services.AddScoped<IBaseDbService<DriverLicense>, DriverLicenseDbService>();
        services.AddScoped<IBaseDbService<Horse>, HorseDbService>();
        services.AddScoped<IBaseDbService<Racecourse>, RaceCourseDbService>();
        services.AddScoped<IBaseDbService<Race>, RaceDbService>();
        services.AddScoped<IBaseDbService<RaceResult>, RaceResultDbService>();
        services.AddScoped<IBaseDbService<RaceStartNumber>, RaceStartNumberDbService>();
        
        return services;
    }
}