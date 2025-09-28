using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class DriverDataService(
    IBaseDbService<Driver> dbService, 
    IBaseCacheService<Driver> cacheService) 
    : BaseDataService<Driver>(dbService, cacheService)
{ }