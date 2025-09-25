using Dapper;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence;

public class BaseDbService<TModel>(DbConnectionStrings dbConnectionStrings) : DbConnection(dbConnectionStrings), IBaseDbService<TModel>
{
    protected string Query { get; init; }
    protected string InsertCommand { get; init; }


    public async Task<List<TModel>> GetAllAsync()
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<TModel>(Query);
        return result.ToList();
    }

    public virtual Task<Dictionary<string, string>> GetIdDictionary()
    {
        throw new NotImplementedException();
    }

    public async Task CreateAsync(TModel model)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(InsertCommand, model);
    }
}