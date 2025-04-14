using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels;

public class EngineSpec : ValueObject
{
    public float VolumeLiters { get; private set; }
    public FuelType FuelType { get; private set; }

    private EngineSpec() { }

    public EngineSpec(float volumeLiters, FuelType fuelType)
    {
        VolumeLiters = volumeLiters;
        FuelType = fuelType;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return VolumeLiters;
        yield return FuelType;
    }
}

