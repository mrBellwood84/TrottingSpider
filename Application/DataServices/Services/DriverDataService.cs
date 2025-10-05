using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class DriverDataService(
    IBaseDbService<Driver> dbService,
    IDriverDbServiceExtension dbServiceExtension,
    IBufferDbService bufferDbService,
    IBaseCacheService<Driver> cacheService)
    : BaseDataService<Driver>(dbService, cacheService), IDriverDataService
{
    private readonly IBaseCacheService<Driver> _cacheService = cacheService;
    private readonly IBaseDbService<Driver> _dbService = dbService;

    public async Task<List<Driver>> GetDriverFromDb(string sourceId)
    {
        var data = await dbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count > 0) _cacheService.AddSingle(data[0]);
        return data;
    }

    public async Task AddDriverToCacheAsync(string sourceId)
    {
        var data = await dbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count == 0) return;
        _cacheService.AddSingle(data[0]);
    }

    public async Task InitDriverCacheAsync()
    {
        var bufferData = await bufferDbService.GetAllDriversAsync();
        var bufferSourceList = bufferData
            .Where(x => x.Collected)
            .Select(x => x.SourceId).ToList();

        var driverData = await _dbService.GetAllAsync();
        var collectedData = driverData
            .Where(x =>  bufferSourceList.Contains(x.SourceId))
            .ToList();
        _cacheService.InitCache(collectedData);
    }
}