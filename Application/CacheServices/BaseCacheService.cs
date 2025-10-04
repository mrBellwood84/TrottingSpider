using Application.CacheServices.Interfaces;
using Models.DbModels;

namespace Application.CacheServices;

/// <summary>
/// Cache service holds db model ids based on provided key
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class BaseCacheService<TModel> : IBaseCacheService<TModel>
{
    private readonly Dictionary<string, TModel> _cache = [];

    /// <summary>
    /// Adds a single key value pair to cache dictionary
    /// </summary>
    public void AddSingle(TModel model)
    {
        var key = CreateKey(model);
        _cache[key] = model;
    }

    /// <summary>
    /// Add range of key value pairs to cache dictionary
    /// </summary>
    /// <param name="models"></param>
    public void AddRange(IEnumerable<TModel> models)
    {
        foreach (var model in models) AddSingle(model);
    }
    
    /// <summary>
    /// Clear cache before adding list of models
    /// </summary>
    /// <param name="models"></param>
    public void InitCache(IEnumerable<TModel> models)
    {
        _cache.Clear();
        AddRange(models);
    }

    /// <summary>
    /// Create key from provided dataset
    /// </summary>
    public virtual string CreateKey(TModel model)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if provided key exists in cache dictionary
    /// </summary>
    public bool CheckKeyExists(string key)
    {
        return _cache.ContainsKey(key);
    }

    /// <summary>
    /// Get model stored in cache from  defined key
    /// </summary>
    public TModel GetModel(string key)
    {
        return CheckKeyExists(key) ? _cache[key] : throw new Exception("Key not found");
    }

    public Dictionary<string, TModel> GetFullCache()
    {
        return _cache;
    }
    
    /// <summary>
    /// Return true if dictionary have no entries
    /// </summary>
    /// <returns></returns>
    public bool CheckDictionaryEmpty()
    {
        return _cache.Count == 0;
    }
}
