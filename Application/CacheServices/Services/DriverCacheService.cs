using Models.DbModels;

namespace Application.CacheServices.Services;

public class DriverCacheService : BaseCacheService<Driver>
{
    public override string CreateKey(Driver model) => model.SourceId;
}