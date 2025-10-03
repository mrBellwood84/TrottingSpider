using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class DriverDataService(
    IBaseDbService<Driver> dbService,
    IDriverDbServiceExtension dbServiceExtension,
    IBaseCacheService<Driver> cacheService)
    : BaseDataService<Driver>(dbService, cacheService), IDriverDataService
{
    private readonly IBaseCacheService<Driver> _cacheService = cacheService;

    public async Task<List<Driver>> GetDriverFromDb(string sourceId)
    {
        var data = await dbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count > 0) _cacheService.AddSingle(data[0]);
        return data;
    }
}