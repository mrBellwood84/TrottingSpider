using Models.DbModels;

namespace Application.CacheServices.Services;

public class RaceStartNumberCacheService : BaseCacheService<RaceStartNumber>
{
    public override string CreateKey(RaceStartNumber model) => $"{model.RaceId}_{model.ProgramNumber}";
}