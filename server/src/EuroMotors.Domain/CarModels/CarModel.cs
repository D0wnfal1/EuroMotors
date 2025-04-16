using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.CarModels;

public class CarModel : Entity
{
    private CarModel()
    {

    }

    public string Brand { get; private set; } = null!;

    public string Model { get; private set; } = null!;

    public int StartYear { get; private set; }

    public BodyType BodyType { get; private set; }

    public EngineSpec EngineSpec { get; private set; }

    public Slug Slug { get; private set; }

    public string? ImagePath { get; private set; }

    public List<Product> Products { get; private set; } = [];

    public static CarModel Create(
        string brand,
        string model,
        int startYear,
        BodyType bodyType,
        EngineSpec engineSpec)
    {
        var car = new CarModel
        {
            Id = Guid.NewGuid(),
            Brand = brand,
            Model = model,
            StartYear = startYear,
            BodyType = bodyType,
            EngineSpec = engineSpec,
            Slug = Slug.GenerateSlug($"{brand}-{model}")
        };

        car.RaiseDomainEvent(new CarModelCreatedDomainEvent(car.Id));

        return car;
    }

    public void Update(string brand, string model, int? startYear = null, BodyType? bodyType = null)
    {
        if (!string.IsNullOrWhiteSpace(brand) && Brand != brand)
        {
            Brand = brand;
            Slug = Slug.GenerateSlug($"{Brand}-{Model}");
            RaiseDomainEvent(new CarModelBrandChangedDomainEvent(Id, Brand));
        }

        if (!string.IsNullOrWhiteSpace(model) && Model != model)
        {
            Model = model;
            Slug = Slug.GenerateSlug($"{Brand}-{Model}");
            RaiseDomainEvent(new CarModelModelChangedDomainEvent(Id, Model));
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


    public Result SetImagePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure(CarModelErrors.InvalidPath(path));
        }
        ImagePath = path;

        RaiseDomainEvent(new CarModelImageUpdatedDomainEvent(Id));

        return Result.Success();
    }
}

