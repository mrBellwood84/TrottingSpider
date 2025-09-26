/*using Models.DbModels;
using Models.ScrapeData;
*/
namespace Scraping.Processors;

public class ProcessResultScrapeData
{
    /*
    private Dictionary<string, string> RaceCourses { get; set;}
    private Dictionary<string, string> Competitions { get; set; }
    private Dictionary<string, string> Races { get; set; }
    private Dictionary<string, string> RaceStartLists { get; set; }
    private Dictionary<string, string> RaceResults { get; set; }
    private Dictionary<string, string> Drivers { get; set; }
    private Dictionary<string, string> Horses { get; set; }
    
    public RaceCourse? NewRaceCourse { get; set; } = null;

    public ProcessResultScrapeData(
        Dictionary<string, string> raceCourse,
        Dictionary<string, string> competitions,
        Dictionary<string, string> races,
        Dictionary<string, string> raceStartLists,
        Dictionary<string, string> raceResults)
    {
        RaceCourses = raceCourse;
        Competitions = competitions;
        Races = races;
        RaceStartLists = raceStartLists;
        RaceResults = raceResults;
    }

    public void Process(ResultScrapeData data)
    {
        var date = _reformatDate(data.Date);
        var raceCourseId = _resolveRacecourseId(data.RaceCourse);
    }

    private string _reformatDate(string date)
    {
        var split = date.Split(".");
        if (split.Length == 2)
        {
            Array.Reverse(split);
            return String.Join("-", split);
        }
        Console.WriteLine($"DEV :: Invalid date when processing {date}");
        return date;
    }
    private string _resolveRacecourseId(string raceCourseName)
    {
        var normalized = raceCourseName.ToUpper();
        if (RaceCourses.ContainsKey(normalized)) return RaceCourses[normalized];
        NewRaceCourse = new RaceCourse
        {
            Id = Guid.NewGuid().ToString(),
            Name = raceCourseName
        };
        return NewRaceCourse.Id;
    }

    private int _resolveRaceNumber(string raceNumber)
    {
        Console.WriteLine($"DEV :: Resolving race number {raceNumber}");
        var number = raceNumber.Split("-")[1];
        return int.Parse(number);
    }
    */
}