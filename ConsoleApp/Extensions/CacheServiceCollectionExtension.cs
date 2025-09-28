using Application.CacheServices;
using Application.CacheServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;

namespace ConsoleApp.Extensions;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<IBaseCacheService<Competition>, CompetitionCacheService>();
        services.AddSingleton<IBaseCacheService<DriverLicense>, DriverLicenseCacheService>();
        services.AddSingleton<IBaseCacheService<Driver>, DriverCacheService>();
        services.AddSingleton<IBaseCacheService<Horse>, HorseCacheService>();
        services.AddSingleton<IBaseCacheService<Race>, RaceCacheService>();
        services.AddSingleton<IBaseCacheService<Racecourse>, RacecourseCacheService>();
        services.AddSingleton<IBaseCacheService<RaceResult>, RaceResultCacheService>();
        services.AddSingleton<IBaseCacheService<RaceStartNumber>, RaceStartNumberCacheService>();
        
        return services;
    }
}