using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Brand.Events;
using EuroMotors.Domain.Category.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Brand;

public class CarModel : Entity
{
    private CarModel(Guid id,
        string brand,
        string model)
        : base(id)
    {
        Brand = brand;
        Model = model;
    }

    public string Brand { get; private set; }

    public string Model { get; private set; }


    public static CarModel Create(string brand, string model, string year)
    {
        var carModel = new CarModel(Guid.NewGuid(), brand, model);

        carModel.RaiseDomainEvents(new CarModelCreatedDomainEvent(carModel.Id));

        return carModel;
    }

    public void ChangeBrand(string brand)
    {
        if (Brand == brand)
        {
            return;
        }

        Brand = brand;

        RaiseDomainEvents(new CategoryNameChangedDomainEvent(Id, Brand));
    }

    public void ChangeModel(string model)
    {
        if (Model == model)
        {
            return;
        }

        Model = model;

        RaiseDomainEvents(new CategoryNameChangedDomainEvent(Id, Model));
    }
}

