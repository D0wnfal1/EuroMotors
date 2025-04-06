using System.Globalization;
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
    public int? EndYear { get; private set; }

    public BodyType BodyType { get; private set; }
    public EngineSpec EngineSpec { get; private set; }

    public Slug Slug { get; private set; }

    public string? ImagePath { get; private set; }

    public List<Product> Products { get; private set; } = [];

    public static CarModel Create(
        string brand,
        string model,
        int startYear,
        int? endYear,
        BodyType bodyType,
        EngineSpec engineSpec)
    {
        var car = new CarModel
        {
            Id = Guid.NewGuid(),
            Brand = brand,
            Model = model,
            StartYear = startYear,
            EndYear = endYear,
            BodyType = bodyType,
            EngineSpec = engineSpec,
            Slug = Slug.GenerateSlug($"{brand}-{model}")
        };

        car.RaiseDomainEvent(new CarModelCreatedDomainEvent(car.Id));

        return car;
    }

    public string GetFullName()
        => $"{Brand} {Model} ({StartYear}-{EndYear?.ToString(CultureInfo.InvariantCulture) ?? "present"})";

    public void Update(string brand, string model, int? startYear = null, int? endYear = null, BodyType? bodyType = null)
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

        if (endYear.HasValue && endYear != EndYear)
        {
            EndYear = endYear.Value;
            RaiseDomainEvent(new CarModelEndYearChangedDomainEvent(Id, EndYear));
        }

        if (bodyType.HasValue && bodyType != BodyType)
        {
            BodyType = bodyType.Value;
            RaiseDomainEvent(new CarModelBodyTypeChangedDomainEvent(Id, BodyType));
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

