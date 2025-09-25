using ConsoleApp.Extensions;
using ConsoleApp.Pipelines;
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
        var dbSettings = configuration.GetSection("ConnectionStrings").Get<DbConnectionStrings>();
        var scraperSettings = configuration.GetSection("Urls").Get<SpiderUrlCollection>();
        
        // create services collection and add settings
        var services = new ServiceCollection();
        services.AddSingleton(dbSettings!);
        services.AddSingleton(scraperSettings!);
        
        // add services from extensions
        services.AddDbServices();
        services.AddCacheServices();
        
        // add pipeline
        services.AddTransient<TestPipeline>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task Run()
    {
        var pipeline = _serviceProvider.GetService<TestPipeline>();
        await pipeline!.Run();
    }

}