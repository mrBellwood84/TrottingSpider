using Dapper;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence;

public class BaseDbService<TModel>(DbConnectionStrings dbConnectionStrings) : DbConnection(dbConnectionStrings), IBaseDbService<TModel>
{
    
    internal string Query { get; init; }
    internal string InsertCommand { get; init; }


    public async Task<List<TModel>> GetAllAsync()
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<TModel>(Query);
        return result.ToList();
    }

    public async Task CreateAsync(TModel model)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(InsertCommand, model);
    }
}