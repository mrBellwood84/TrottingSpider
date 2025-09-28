using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class DriverLicenseDataService(
    IBaseDbService<DriverLicense> dbService, 
    IBaseCacheService<DriverLicense> cacheService) 
    : BaseDataService<DriverLicense>(dbService, cacheService)
{ }