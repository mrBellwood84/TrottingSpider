using Models.DbModels;

namespace Application.CacheServices;

public class DriverLicenseCacheService :  BaseCacheService<DriverLicense>
{
    public override string CreateKey(DriverLicense data) => data.Code;
}