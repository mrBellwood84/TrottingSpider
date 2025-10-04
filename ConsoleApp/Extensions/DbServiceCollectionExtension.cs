using Microsoft.Extensions.DependencyInjection;
using Models.DbModels;
using Persistence.Interfaces;
using Persistence.Services;
using Persistence.Services.Extensions;

namespace ConsoleApp.Extensions;

public static class DbServiceCollectionExtension
{
    public static IServiceCollection AddDbServices(this IServiceCollection services)
    {
        // add scoped db services
        services.AddScoped<IBufferDbService, BufferDbService>();
        services.AddScoped<IBaseDbService<Competition>, CompetitionDbService>();
        services.AddScoped<IBaseDbService<Driver>, DriverDbService>();
        services.AddScoped<IBaseDbService<DriverLicense>, DriverLicenseDbService>();
        services.AddScoped<IBaseDbService<Horse>, HorseDbService>();
        services.AddScoped<IBaseDbService<Racecourse>, RaceCourseDbService>();
        services.AddScoped<IBaseDbService<Race>, RaceDbService>();
        services.AddScoped<IBaseDbService<RaceResult>, RaceResultDbService>();
        services.AddScoped<IBaseDbService<RaceStartNumber>, RaceStartNumberDbService>();
        
        // add scoped db service extensions
        services.AddScoped<IDriverDbServiceExtension, DriverDbServiceExtension>();
        services.AddScoped<IHorseDbServiceExtension, HorseDbServiceExtension>();
        services.AddScoped<IRaceResultsDbServiceExtension, RaceResultsDbServiceExtension>();
        services.AddScoped<IRaceStartNumberDbServiceExtension, RaceStartNumberDbServiceExtension>();
        
        return services;
    }
}