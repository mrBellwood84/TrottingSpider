using Application.CacheServices;
using Models.DbModels;
using Persistence.Interfaces;

namespace ConsoleApp.Pipelines;

public class TestPipeline : ITestPipeline
{
    private readonly IBaseDbService<DriverLicense> _dbService; 
    private readonly DriverLicenseCacheService _cacheService;
    
    public TestPipeline(IBaseDbService<DriverLicense> dbService, DriverLicenseCacheService  cacheService)
    {
        _dbService = dbService;
        _cacheService = cacheService;
    }

    public async Task Run()
    {
        var data = await _dbService.GetAllAsync();
        var cache = _cacheService;
        cache.AddRange(data);

        var real = data[0]!;

        var mock = new DriverLicense
        {
            Id = Guid.NewGuid().ToString(),
            Code = "Lol",
            Description = "This is mock"
        };

        var realStringCheck = cache.CheckKeyExists(real.Code);
        var realModelCheck = cache.CheckKeyExists(real);
        var realIdExtracted = cache.GetModel(cache.CreateKey(real));
        
        var mockStringCheckBeforeAdd = cache.CheckKeyExists(mock.Code);
        var mockModelCheckBeforeAdd = cache.CheckKeyExists(mock);
        // var mockIdExtractedBeforeAdd = cache.GetModel(cache.CreateKey(mock));
        
        cache.AddSingle(mock);
        
        var mockStringCheckAfterAdd = cache.CheckKeyExists(mock.Code);
        var mockModelCheckAfterAdd = cache.CheckKeyExists(mock);
        var mockIdExtractedAfterAdd = cache.GetModel(cache.CreateKey(mock));
        
        Console.WriteLine($"\n\nReal data item id: {real.Id}");
        Console.WriteLine($"Check real key directly: {realStringCheck}");
        Console.WriteLine($"Check model key: {realModelCheck}");
        Console.WriteLine($"Check id extracted: {realIdExtracted.Id == real.Id}");

        Console.WriteLine($"\nCheck mock key before add directly: {mockStringCheckBeforeAdd}");
        Console.WriteLine($"Check mock key before add model: {mockModelCheckBeforeAdd}");
        // Console.WriteLine($"Check mock id  extracted before add: {mockIdExtractedBeforeAdd.Id}");
        
        Console.WriteLine($"\nCheck mock key after add: {mockStringCheckAfterAdd}");
        Console.WriteLine($"Check mock model after add: {mockModelCheckAfterAdd}");
        Console.WriteLine($"Check mock id extracted after add: {mockIdExtractedAfterAdd.Id}");
        Console.WriteLine($"Mock id as stored in object:       {mock.Id}");
        Console.WriteLine($"Check id similar: {mockIdExtractedAfterAdd.Id == mock.Id}");
        
        
    }
    
}