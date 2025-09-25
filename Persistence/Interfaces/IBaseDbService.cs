namespace Persistence.Interfaces;

public interface IBaseDbService<TModel>
{
    Task<List<TModel>> GetAllAsync();
    Task<Dictionary<string, string>> GetIdDictionary();
    Task CreateAsync(TModel model);
}