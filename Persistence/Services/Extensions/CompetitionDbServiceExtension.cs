using Dapper;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class CompetitionDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), ICompetitionDbServiceExtension
{
    private readonly string _updateStartlistFromSource = 
        @"UPDATE Competition SET startlistFromSource = @StartlistFromSource WHERE Id = @Id";
    private readonly string _updateResultsFromSource = 
        @"UPDATE Competition SET resultsFromSource = @ResultsFromSource WHERE Id = @Id";

    public async Task UpdateStartListFromSource(CompetitionUpdateStartlistSource data)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateStartlistFromSource, data);
    }

    public async Task UpdateResultsFromSource(CompetitionUpdateResultSource data)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateResultsFromSource, data);
    }
}