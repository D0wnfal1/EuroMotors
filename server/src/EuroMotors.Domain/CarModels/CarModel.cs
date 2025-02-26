using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels.Events;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.CarModels;

public class CarModel : Entity
{
    private CarModel()
    {

    }

    public string Brand { get; private set; }

    public string Model { get; private set; }

    public Uri? ImageUrl { get; private set; }

    public List<Product> Products { get; private set; } = [];

    public static CarModel Create(string brand, string model)
    {
        var carModel = new CarModel()
        {
            Id = Guid.NewGuid(),
            Brand = brand,
            Model = model,
        };

        carModel.RaiseDomainEvent(new CarModelCreatedDomainEvent(carModel.Id));

        return carModel;
    }

    public void Update(string brand, string model)
    {
        if (string.IsNullOrWhiteSpace(brand) || Brand == brand)
        {
            return;
        }

        Brand = brand;

        RaiseDomainEvent(new CarModelBrandChangedDomainEvent(Id, Brand));

        if (string.IsNullOrWhiteSpace(model) || Model == model)
        {
            return;
        }

        Model = model;

        RaiseDomainEvent(new CarModelModelChangedDomainEvent(Id, Model));
    }
    public Result UpdateImage(Uri newUrl)
    {
        if (string.IsNullOrWhiteSpace(newUrl.ToString()))
        {
            return Result.Failure(CarModelErrors.InvalidUrl(newUrl));
        }

        ImageUrl = newUrl;

        RaiseDomainEvent(new CarModelImageUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result DeleteImage()
    {
        if (Id == Guid.Empty)
        {
            return Result.Failure(CarModelErrors.NotFound(Id));
        }

        ImageUrl = null;

        RaiseDomainEvent(new CarModelImageDeletedDomainEvent(Id));

        return Result.Success();
    }
}

