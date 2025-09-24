using Microsoft.Extensions.Configuration;

namespace ConsoleApp;

public class AppConfigurations
{
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();
    
    public string? ConnectionString => _configuration.GetValue<string>("Database:ConnectionString:DefaultConnection");
}