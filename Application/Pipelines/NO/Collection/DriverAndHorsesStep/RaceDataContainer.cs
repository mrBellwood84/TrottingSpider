using Models.DbModels;
using Models.DbModels.Updates;

namespace Application.Pipelines.NO.Collection.DriverAndHorsesStep;

public class RaceDataContainer
{
    public Dictionary<string, Competition>  Competitions { get; } = [];
    public Dictionary<string, Race> Races { get; } = [];
    public Dictionary<string, RaceStartNumber> AllStartNumbers { get; } = [];
    public HashSet<string> RaceResultKeys { get; } = [];

    public List<RaceStartNumber> StartNumbersCreate { get; } = [];
    public List<RaceStartNumberUpdateDriver> StartNumbersUpdateDriver { get; } = [];
    public List<RaceStartNumberUpdateHorse> StartNumbersUpdateHorse { get; } = [];
    public List<RaceResult>  RaceResultsCreate { get; } = [];
    
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