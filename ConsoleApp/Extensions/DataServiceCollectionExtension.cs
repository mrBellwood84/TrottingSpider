using Application.DataServices;
using Application.DataServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Extensions;

public static class DataServiceCollectionExtension
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // add individual services
        services.AddScoped<ICompetitionDataService, CompetitionDataService>();
        services.AddScoped<IRaceCourseDataService, RaceCourseDataService>();
        
        // add collection
        services.AddScoped<IDataServiceCollection,  DataServiceCollection>();
        
        return services;
    }
}