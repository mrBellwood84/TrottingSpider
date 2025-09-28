using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices;

public class HorseDataService(
    IBaseDbService<Horse> dbService, 
    IBaseCacheService<Horse> cacheService) 
    : BaseDataService<Horse>(dbService, cacheService)
{ }