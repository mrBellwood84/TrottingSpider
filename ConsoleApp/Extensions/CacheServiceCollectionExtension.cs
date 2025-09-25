using Application.CacheServices;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Extensions;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<CompetitionCacheService>();
        services.AddSingleton<DriverLicenseCacheService>();
        services.AddSingleton<DriverCacheService>();
        services.AddSingleton<HorseCacheService>();
        services.AddSingleton<RaceCacheService>();
        services.AddSingleton<RaceCourseCacheService>();
        services.AddSingleton<RaceResultCacheService>();
        services.AddSingleton<RaceStartNumberCacheService>();
        
        return services;
    }
}