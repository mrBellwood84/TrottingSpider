using Application.CacheServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class RaceResultDataService(
    IBaseDbService<RaceResult> dbService,
    IRaceResultsDbServiceExtension raceResultDbServiceExtension,
    IBaseCacheService<RaceResult> cacheService)
    : BaseDataService<RaceResult>(dbService, cacheService), IRaceResultDataService
{

    public async Task UpdateAsync(RaceResultUpdate data) => await raceResultDbServiceExtension.UpdateAsync(data);
}