using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class DriverDataService(
    IBaseDbService<Driver> dbService,
    IBaseCacheService<Driver> cacheService)
    : BaseDataService<Driver>(dbService, cacheService)
{
    
}