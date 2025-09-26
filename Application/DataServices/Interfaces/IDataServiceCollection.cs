namespace Application.DataServices.Interfaces;

public interface IDataServiceCollection
{
    ICompetitionDataService Competition { get; }
    IRaceCourseDataService RaceCourse { get; }
    Task InitializeCache();
}