using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands.Events;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Domain.CarBrands;

public sealed class CarBrand : Entity
{
    private CarBrand()
    {
    }

    public string Name { get; private set; } = null!;

    public Slug Slug { get; private set; }

    public string? LogoPath { get; private set; }

    public List<CarModel> Models { get; private set; } = [];

    public static CarBrand Create(string name)
    {
        var carBrand = new CarBrand
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = Slug.GenerateSlug(name)
        };

        carBrand.RaiseDomainEvent(new CarBrandCreatedDomainEvent(carBrand.Id));

        return carBrand;
    }

    public void Update(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && Name != name)
        {
            Name = name;
            Slug = Slug.GenerateSlug(name);
            RaiseDomainEvent(new CarBrandNameChangedDomainEvent(Id, Name));
        }
    }

    public Result SetLogoPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure(CarModelErrors.InvalidPath(path));
        }

        LogoPath = path;
        RaiseDomainEvent(new CarBrandLogoUpdatedDomainEvent(Id));

        return Result.Success();
    }
}
