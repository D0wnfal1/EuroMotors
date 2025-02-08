namespace EuroMotors.Domain.Brand;

public interface ICarModelRepository
{
    Task<CarModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(CarModel carModel);
}
