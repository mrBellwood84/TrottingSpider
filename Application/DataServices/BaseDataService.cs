using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Persistence.Interfaces;

namespace Application.DataServices;

public class BaseDataService<TModel>(
    IBaseDbService<TModel> dbService, 
    IBaseCacheService<TModel> cacheService) : IBaseDataService<TModel>
{
    public async Task InitCache()
    {
        var data = await dbService.GetAllAsync();
        cacheService.InitCache(data!);
    }

    public bool CheckExists(string key) => cacheService.CheckKeyExists(key);

    public TModel GetModel(string key) => cacheService.GetModel(key);

    public async Task CreateAsync(TModel model)
    {
        await dbService.CreateAsync(model);
        cacheService.AddSingle(model);
    }
}
    
