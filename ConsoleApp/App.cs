using Application.Pipelines.NO;
using ConsoleApp.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Settings;

namespace ConsoleApp;

public class App
{
    private readonly IServiceProvider _serviceProvider;
    
    public App()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        // get settings from config file
        var browserOptions = configuration.GetSection("BrowserOptions").Get<BrowserOptions>();
        var dbSettings = configuration.GetSection("ConnectionStrings").Get<DbConnectionStrings>();
        var scraperSettings = configuration.GetSection("ScraperSettings").Get<ScraperSettings>();
        
        // create services collection and add settings
        var services = new ServiceCollection();
        services.AddSingleton(browserOptions!);
        services.AddSingleton(dbSettings!);
        services.AddSingleton(scraperSettings!);
        
        // add services from extensions
        services.AddCacheServices();
        services.AddDbServices();
        services.AddDataServices();
        
        //
        
        // add pipeline
        services.AddTransient<Pipeline>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public async Task RunTrottingNoAsync()
    {
        var pipeline = _serviceProvider.GetService<Pipeline>();
        await pipeline!.RunAsync();
    }


    
}