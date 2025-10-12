using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface ICompetitionDbServiceExtension
{
    Task UpdateStartListFromSource(CompetitionUpdateStartlistSource data);
    Task UpdateResultsFromSource(CompetitionUpdateResultSource data);
}