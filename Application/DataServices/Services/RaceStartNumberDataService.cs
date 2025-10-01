using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Persistence.Interfaces;
using Persistence.Services.Extensions;

namespace Application.DataServices.Services;

public class RaceStartNumberDataService(
    IBaseDbService<RaceStartNumber> dbService,
    IBaseCacheService<RaceStartNumber> cacheService,
    IRaceStartNumberDbServiceExtension dbServiceExtension) :
    BaseDataService<RaceStartNumber>(dbService, cacheService), IRaceStartNumberDataService
{
    public async Task UpdateDriverAsync(RaceStartNumberUpdateDriver data) =>  await dbServiceExtension.UpdateDriverAsync(data);
    public async Task UpdateHorseAsync(RaceStartNumberUpdateHorse data) =>  await dbServiceExtension.UpdateHorseAsync(data);
    public async Task UpdateAsync(RaceStartNumberUpdate data) =>  await dbServiceExtension.UpdateAsync(data);
}