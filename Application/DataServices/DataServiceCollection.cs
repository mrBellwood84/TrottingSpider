using Application.DataServices.Interfaces;

namespace Application.DataServices;

public class DataServiceCollection : IDataServiceCollection
{
    public ICompetitionDataService Competition { get; }
    public IRaceCourseDataService RaceCourse { get; }
    

    public DataServiceCollection(
        ICompetitionDataService  competitionDataService, 
        IRaceCourseDataService raceCourseDataService )
    {
        Competition = competitionDataService;
        RaceCourse = raceCourseDataService;
    }

    public async Task InitializeCache()
    {
        await Competition.InitCache();
        await RaceCourse.InitCache();
    }
}