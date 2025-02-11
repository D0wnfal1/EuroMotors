namespace EuroMotors.Domain.CarModels;

public interface ICarModelRepository
{
    Task<CarModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(CarModel carModel);

    void Delete(Guid carModelId);
}
