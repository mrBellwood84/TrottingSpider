using Models.DbModels;

namespace Application.CacheServices;

public class DriverCacheService : BaseCacheService<Driver>
{
    public new string CreateKey(Driver model) => model.SourceId;
}