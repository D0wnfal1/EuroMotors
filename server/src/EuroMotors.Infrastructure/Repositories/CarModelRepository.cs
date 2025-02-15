using EuroMotors.Domain.CarModels;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CarModelRepository : Repository<CarModel>, ICarModelRepository
{
    public CarModelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async void Delete(Guid carModelId)
    {
        CarModel? carModel = await GetByIdAsync(carModelId);

        if (carModel == null)
        {
            return;
        }

        _dbContext.CarModels.Remove(carModel);

        await _dbContext.SaveChangesAsync();
    }
}
