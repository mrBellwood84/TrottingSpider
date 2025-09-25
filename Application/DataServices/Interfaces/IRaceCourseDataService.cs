using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IRaceCourseDataService
{
    Task InitCache();
    bool CheckRaceCourseExists(string raceCourseName);
    bool CheckRaceCourseExists(RaceCourse raceCourse);
    RaceCourse GetRaceCourse(string raceCourseName);
    Task AddNewRaceCourse(RaceCourse newRaceCourse);
}