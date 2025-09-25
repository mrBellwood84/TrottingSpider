using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface ICompetitionDataService
{
    Task InitCache();
    bool CheckCompetitionExists(string key);
    bool CheckCompetitionExists(string raceCourseId, string date);
    bool CheckCompetitionExists(Competition data);
    Competition GetCompetition(string key);
    Task AddNewCompetition(Competition newCompetition);
}