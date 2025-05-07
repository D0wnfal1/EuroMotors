using EuroMotors.Domain.CarModels;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CarModelRepository : Repository<CarModel>, ICarModelRepository
{
    public CarModelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<CarModel> GetAll()
    {
        return _dbContext.Set<CarModel>();
    }

    public async Task Delete(Guid carModelId)
    {
        CarModel? carModel = await GetByIdAsync(carModelId);

        if (carModel is null)
        {
            return;
        }

        _dbContext.Set<CarModel>().Remove(carModel);

        await _dbContext.SaveChangesAsync();
    }
}
