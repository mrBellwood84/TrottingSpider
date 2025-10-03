using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Persistence.Interfaces;
using Persistence.Services.Extensions;

namespace Application.DataServices.Services;

public class RaceStartNumberDataService(
    IBaseDbService<RaceStartNumber> dbService,
    IRaceStartNumberDbServiceExtension dbServiceExtension,
    IBaseCacheService<RaceStartNumber> cacheService) :
    BaseDataService<RaceStartNumber>(dbService, cacheService), IRaceStartNumberDataService
{
    private readonly IBaseCacheService<RaceStartNumber> _cacheService = cacheService;


    public async Task AddBulkAsync(List<RaceStartNumber> data)
    {
        await dbServiceExtension.InsertBulkAsync(data);
        _cacheService.AddRange(data);
    }

    public async Task UpdateAsync(RaceStartNumberUpdate data) => await dbServiceExtension.UpdateAsync(data);

    public async Task UpdateDriversBulkAsync(List<RaceStartNumberUpdateDriver> data)
    {
        await dbServiceExtension.UpdateBulkDriversAsync(data);
    }

    public async Task UpdateHorsesBulkAsync(List<RaceStartNumberUpdateHorse> data)
    {
        await dbServiceExtension.UpdateHorsesBulkAsync(data);
    }
    
}