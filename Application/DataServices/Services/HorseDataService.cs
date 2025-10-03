using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class HorseDataService(
    IBaseDbService<Horse> dbService,
    IHorseDbServiceExtension horseDbServiceExtension,
    IBaseCacheService<Horse> cacheService)
    : BaseDataService<Horse>(dbService, cacheService), IHorseDataService
{
    private readonly IBaseCacheService<Horse> _cacheService = cacheService;

    public async Task<List<Horse>> GetHorseFromDb(string sourceId)
    {
        var data = await horseDbServiceExtension.QueryBySourceId(sourceId);
        if (data.Count > 0) _cacheService.AddSingle(data[0]);
        return data;
    }
}