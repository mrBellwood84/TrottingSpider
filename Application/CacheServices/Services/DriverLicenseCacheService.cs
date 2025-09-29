using Models.DbModels;

namespace Application.CacheServices.Services;

public class DriverLicenseCacheService :  BaseCacheService<DriverLicense>
{
    public override string CreateKey(DriverLicense data) => data.Code;
}