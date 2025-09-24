using Microsoft.Extensions.Configuration;
using M
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
    

namespace Program;

public class App
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}