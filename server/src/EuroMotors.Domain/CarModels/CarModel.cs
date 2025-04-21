using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.CarModels;

public sealed class CarModel : Entity
{
    private CarModel()
    {
    }

    public Guid CarBrandId { get; private set; }

    public CarBrand? CarBrand { get; private set; }

    public string ModelName { get; private set; } = null!;

    public int StartYear { get; private set; }

    public BodyType BodyType { get; private set; }

    public EngineSpec EngineSpec { get; private set; }

    public Slug Slug { get; private set; }

    public List<Product> Products { get; private set; } = [];

    public static CarModel Create(
        CarBrand carBrand,
        string modelName,
        int startYear,
        BodyType bodyType,
        EngineSpec engineSpec)
    {
        var car = new CarModel
        {
            Id = Guid.NewGuid(),
            CarBrandId = carBrand.Id,
            CarBrand = carBrand,
            ModelName = modelName,
            StartYear = startYear,
            BodyType = bodyType,
            EngineSpec = engineSpec,
            Slug = Slug.GenerateSlug($"{carBrand.Name}-{modelName}")
        };

        car.RaiseDomainEvent(new CarModelCreatedDomainEvent(car.Id));

        return car;
    }

    public void Update(string modelName, int? startYear = null, BodyType? bodyType = null)
    {
        if (!string.IsNullOrWhiteSpace(modelName) && ModelName != modelName)
        {
            ModelName = modelName;
            if (CarBrand is not null)
            {
                Slug = Slug.GenerateSlug($"{CarBrand.Name}-{ModelName}");
            }
            RaiseDomainEvent(new CarModelModelChangedDomainEvent(Id, ModelName));
        }

        if (startYear.HasValue && startYear != StartYear)
        {
            StartYear = startYear.Value;
            RaiseDomainEvent(new CarModelStartYearChangedDomainEvent(Id, StartYear));
        }

        if (bodyType.HasValue && bodyType != BodyType)
        {
            BodyType = bodyType.Value;
            RaiseDomainEvent(new CarModelBodyTypeChangedDomainEvent(Id, BodyType));
        }
    }

    public void UpdateEngineSpec(float? volumeLiters, FuelType? fuelType)
    {
        bool shouldUpdate = false;

        float newVolume = EngineSpec.VolumeLiters;
        FuelType newFuel = EngineSpec.FuelType;

        const float tolerance = 0.0001f;

        if (volumeLiters.HasValue && Math.Abs(volumeLiters.Value - EngineSpec.VolumeLiters) > tolerance)
        {
            newVolume = volumeLiters.Value;
            shouldUpdate = true;
        }

        if (fuelType.HasValue && fuelType.Value != EngineSpec.FuelType)
        {
            newFuel = fuelType.Value;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            EngineSpec = new EngineSpec(newVolume, newFuel);
            RaiseDomainEvent(new CarModelEngineSpecUpdatedDomainEvent(Id));
        }
    }
}

