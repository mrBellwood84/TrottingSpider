using Models.DbModels;

namespace Application.CacheServices.Services;

public class RaceResultCacheService : BaseCacheService<RaceResult>
{
    public override string CreateKey(RaceResult model) => model.RaceStartNumberId;
}