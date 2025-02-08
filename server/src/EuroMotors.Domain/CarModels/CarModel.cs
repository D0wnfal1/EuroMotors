using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Brand.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Brand;

public class CarModel : Entity
{
    private CarModel()
    {

    }

    public string Brand { get; private set; }

    public string Model { get; private set; }

    public List<Product> Products { get; private set; } = new();

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

    public void Update(string brand, string model, int year)
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
}

