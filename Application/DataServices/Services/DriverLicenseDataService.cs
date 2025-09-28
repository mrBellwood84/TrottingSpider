using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class DriverLicenseDataService(
    IBaseDbService<DriverLicense> dbService,
    IBaseCacheService<DriverLicense> cacheService)
    : BaseDataService<DriverLicense>(dbService, cacheService), IDriverLicenseDataService
{
    private readonly IBaseCacheService<DriverLicense> _cacheService = cacheService;
    
    public Dictionary<string, DriverLicense> GetFullCache() => _cacheService.GetFullCache();
}