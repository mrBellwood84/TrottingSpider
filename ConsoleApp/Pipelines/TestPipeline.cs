using Models.DbModels;
using Persistence.Interfaces;

namespace ConsoleApp.Pipelines;

public class TestPipeline : ITestPipeline
{
    private readonly IBaseDbService<DriverLicense> _dbService; 
    
    public TestPipeline(IBaseDbService<DriverLicense> dbService)
    {
        _dbService = dbService;        
    }

    public async Task Run()
    {
        var data = await _dbService.GetIdDictionary();
        foreach (var item in data)
        {
            Console.WriteLine($"{item.Key} - {item.Value}");
        }
    }
}