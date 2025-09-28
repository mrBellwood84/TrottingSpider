using Application.CacheServices.Interfaces;
using Models.DbModels;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class CompetitionDataService(
    IBaseDbService<Competition> dbService, 
    IBaseCacheService<Competition> cacheService) 
    : BaseDataService<Competition>(dbService, cacheService)
{ }