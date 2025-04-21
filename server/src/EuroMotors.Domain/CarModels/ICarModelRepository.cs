namespace EuroMotors.Domain.CarModels;

public interface ICarModelRepository
{
    Task<CarModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<CarModel>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default);

    Task<CarModel?> GetByBrandAndModelNameAsync(Guid brandId, string modelName, CancellationToken cancellationToken = default);

    void Insert(CarModel carModel);

    Task Delete(Guid carModelId);
}
