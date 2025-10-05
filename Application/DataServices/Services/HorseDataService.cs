using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class HorseDataService(
    IBaseDbService<Horse> dbService,
    IHorseDbServiceExtension horseDbServiceExtension,
    IBufferDbService bufferDbService,
    IBaseCacheService<Horse> cacheService)
    : BaseDataService<Horse>(dbService, cacheService), IHorseDataService
{
    private readonly IBaseCacheService<Horse> _cacheService = cacheService;
    private readonly IBaseDbService<Horse> _dbService = dbService;

    public async Task<List<Horse>> GetHorseFromDb(string sourceId)
    {
        var data = await horseDbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count > 0) _cacheService.AddSingle(data[0]);
        return data;
    }

    public async Task AddHorseToCacheAsync(string sourceId)
    {
        var data = await horseDbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count == 0) return;
        _cacheService.AddSingle(data[0]);
    }

    public async Task InitHorseCacheAsync()
    {
        var bufferData = await bufferDbService.GetAllDriversAsync();
        var bufferSourceList = bufferData
            .Where(x => x.Collected)
            .Select(x => x.SourceId).ToList();
        
        var horseData = await _dbService.GetAllAsync();
        var collectedData = horseData
            .Where(x => bufferSourceList.Contains(x.SourceId))
            .ToList();
        _cacheService.InitCache(collectedData);
    }
}