using Models.DbModels;

namespace Application.Pipelines.NO.Collection.DriverAndHorsesStep;

internal struct CreateUpdateResult
{
    public RaceStartNumber StartNumberData { get; init; }
    public CreateUpdateOptions Option { get; init; }
}