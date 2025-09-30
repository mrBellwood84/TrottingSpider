using Dapper;
using Models.DbModels.Updates;
using Models.Settings;

namespace Persistence.Services.Extensions;

public class RaceStartNumberDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IRaceStartNumberDbServiceExtension
{
    private readonly string _updateDriverCommand = 
        "UPDATE RaceStartNumber SET DriverId = @DriverId WHERE Id = @Id";

    private readonly string _updateHorseCommand = 
        "UPDATE RaceStartNumber SET HorseId = @HorseId  WHERE Id = @Id";
    
    public async Task UpdateDriverAsync(RaceStartNumberUpdateDriver values)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateDriverCommand, values);
    }

    public async Task UpdateHorseAsync(RaceStartNumberUpdateHorse values)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateHorseCommand, values);
    }
}