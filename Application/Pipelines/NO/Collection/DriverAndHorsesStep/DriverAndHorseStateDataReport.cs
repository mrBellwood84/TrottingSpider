namespace Application.Pipelines.NO.Collection.DriverAndHorsesStep;

internal struct DriverAndHorseStateDataReport
{
    public DriverAndHorseStateDataReport() { }
    public int NewDrivers { get; set; } = 0;
    public int NewHorses { get; set; } = 0;
    public int NewDriverLicenses { get; set; } = 0;
    public int NewRacecourses { get; set; } = 0;
    public int NewCompetition { get; set; } = 0;
    public int NewRaces { get; set; } = 0;
    public int StartnumberCreated { get; set; } = 0;
    public int StartnumberUpdatedDriver { get; set; } = 0;
    public int StartnumberUpdatedHorses { get; set; } = 0;

    public void Report()
    {
        Console.WriteLine("\n\n");
        if (NewDrivers > 0) AppLogger.LogPositive($"Drivers created: {NewDrivers}");
        if (NewHorses > 0) AppLogger.LogPositive($"Horses created: {NewHorses}");
        if (NewDriverLicenses > 0) AppLogger.LogPositive($"Driver licenses created: {NewDriverLicenses}");
        if (NewRacecourses > 0) AppLogger.LogPositive($"Racecourses created: {NewRacecourses}");
        if (NewCompetition > 0) AppLogger.LogPositive($"Competition created: {NewCompetition}");
        if (NewRaces > 0) AppLogger.LogPositive($"Races created: {NewRaces}");
        if (StartnumberCreated > 0) AppLogger.LogPositive($"New start numbers created: {StartnumberCreated}");
        if (StartnumberUpdatedDriver > 0) AppLogger.LogPositive($"Drivers results updated: {StartnumberUpdatedDriver}");
        if (StartnumberUpdatedHorses > 0) AppLogger.LogPositive($"Horses results updated: {StartnumberUpdatedHorses}");
        
        NewDrivers = 0;
        NewHorses = 0;
        NewDriverLicenses = 0;
        NewRacecourses = 0;
        NewCompetition = 0;
        NewRaces = 0;
        StartnumberCreated = 0;
        StartnumberUpdatedDriver = 0;
        StartnumberUpdatedHorses = 0;
    }
}