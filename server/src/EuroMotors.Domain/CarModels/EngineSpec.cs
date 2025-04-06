using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels;

public class EngineSpec : ValueObject
{
    public float VolumeLiters { get; private set; }
    public FuelType FuelType { get; private set; }
    public int HorsePower { get; private set; }

    private EngineSpec() { }

    public EngineSpec(float volumeLiters, FuelType fuelType, int horsePower)
    {
        VolumeLiters = volumeLiters;
        FuelType = fuelType;
        HorsePower = horsePower;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return VolumeLiters;
        yield return FuelType;
        yield return HorsePower;
    }
}

