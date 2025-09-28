using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class RaceStartNumberDataService(
    IBaseDbService<RaceStartNumber> dbService, 
    IBaseCacheService<RaceStartNumber> cacheService) : 
    BaseDataService<RaceStartNumber>(dbService, cacheService)
{ }