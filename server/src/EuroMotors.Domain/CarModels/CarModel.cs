using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Brand.Events;

namespace EuroMotors.Domain.Brand;

public class CarModel : Entity
{
    private CarModel(Guid id,
        string brand,
        string model,
        string year)
        : base(id)
    {
        Brand = brand;
        Model = model;
        Year = year;
    }

    public string Brand { get; private set; }

    public string Model { get; private set; }

    public string Year { get; private set; }

    public static CarModel Create(string brand, string model, string year)
    {
        var carModel = new CarModel(Guid.NewGuid(), brand, model, year);

        carModel.RaiseDomainEvents(new CarModelCreatedDomainEvent(carModel.Id));

        return carModel;
    }
}

