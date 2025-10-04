using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.Pipelines.NO.Collection.DriverAndHorsesStep;

public class RaceDataContainer
{
    public Dictionary<string, Competition>  Competitions { get; set; } = [];
    public Dictionary<string, Race> Races { get; set; } = [];
    public Dictionary<string, RaceStartNumber> AllStartNumbers { get; set; } = [];
    public HashSet<string> RaceResultKeys { get; set; } = [];

    public List<RaceStartNumber> StartNumbersCreate = [];
    public List<RaceStartNumberUpdateDriver> StartNumbersUpdateDriver = [];
    public List<RaceStartNumberUpdateHorse> StartNumbersUpdateHorse = [];
    public List<RaceResult>  RaceResultsCreate { get; set; } = [];
    
    public void Clear()
    {
        Competitions.Clear();
        Races.Clear();
        AllStartNumbers.Clear();
        RaceResultKeys.Clear();
        StartNumbersCreate.Clear();
        StartNumbersUpdateDriver.Clear();
        StartNumbersUpdateHorse.Clear();
        RaceResultsCreate.Clear();
    }

}