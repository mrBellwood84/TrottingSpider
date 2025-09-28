namespace Application.CacheServices.Interfaces;

public interface IBaseCacheService<TModel>
{
    /// <summary>
    /// Adds a single key value pair to cache dictionary
    /// </summary>
    void AddSingle(TModel model);

    /// <summary>
    /// Add range of key value pairs to cache dictionary
    /// </summary>
    /// <param name="models"></param>
    void AddRange(IEnumerable<TModel> models);

    /// <summary>
    /// Clear cache before adding list of models
    /// </summary>
    /// <param name="models"></param>
    void InitCache(IEnumerable<TModel> models);

    /// <summary>
    /// Create key from provided dataset
    /// </summary>
    string CreateKey(TModel model);

    /// <summary>
    /// Check if provided key exists in cache dictionary
    /// </summary>
    bool CheckKeyExists(string key);

    /// <summary>
    /// Create key from object and check if key exists
    /// </summary>
    bool CheckKeyExists(TModel model);

    /// <summary>
    /// Get model stored in cache from  defined key
    /// </summary>
    TModel GetModel(string key);

    /// <summary>
    /// Return true if dictionary have no entries
    /// </summary>
    /// <returns></returns>
    bool CheckDictionaryEmpty();

    /// <summary>
    /// Get full dictionary from cache
    /// </summary>
    Dictionary<string, TModel> GetFullCache();
}