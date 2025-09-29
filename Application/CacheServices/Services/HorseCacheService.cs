using Models.DbModels;

namespace Application.CacheServices.Services;

public class HorseCacheService : BaseCacheService<Horse>
{
    public override string CreateKey(Horse data) => data.SourceId;
}