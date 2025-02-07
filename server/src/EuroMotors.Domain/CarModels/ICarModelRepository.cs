namespace EuroMotors.Domain.Brand;

public interface ICarModelRepository
{
    Task<CarModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Insert(CarModel carModel);
}
