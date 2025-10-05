using Application.CacheServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class RaceStartNumberDataService(
    IBaseDbService<RaceStartNumber> dbService,
    IRaceStartNumberDbServiceExtension dbServiceExtension,
    IBaseCacheService<RaceStartNumber> cacheService) :
    BaseDataService<RaceStartNumber>(dbService, cacheService), IRaceStartNumberDataService
{
    private readonly IBaseCacheService<RaceStartNumber> _cacheService = cacheService;
    private readonly IBaseDbService<RaceStartNumber> _dbService = dbService;

    public async Task BulkAddAsync(List<RaceStartNumber> data)
    {
        await _dbService.BulkInsertAsync(data);
        _cacheService.AddRange(data);
    }

    public async Task UpdateAsync(RaceStartNumberUpdate data) => await dbServiceExtension.UpdateAsync(data);

    public async Task BulkUpdateDriversAsync(List<RaceStartNumberUpdateDriver> data)
    {
        await dbServiceExtension.BulkUpdateDriversAsync(data);
    }

    public async Task BulkUpdateHorsesAsync(List<RaceStartNumberUpdateHorse> data)
    {
        await dbServiceExtension.BulkUpdateHorsesAsync(data);
    }
    
}