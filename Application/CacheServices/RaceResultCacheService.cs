using Models.DbModels;

namespace Application.CacheServices;

public class RaceResultCacheService : BaseCacheService<RaceResult>
{
    public override string CreateKey(RaceResult model) => model.RaceStartNumberId;
}