namespace EuroMotors.Domain.CarBrands;

public interface ICarBrandRepository
{
    Task<CarBrand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CarBrand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    void Insert(CarBrand carBrand);

    Task Delete(Guid carBrandId);
}