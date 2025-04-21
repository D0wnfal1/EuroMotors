using EuroMotors.Domain.CarModels;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CarModelRepository : Repository<CarModel>, ICarModelRepository
{
    public CarModelRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<CarModel>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CarModel>()
            .Where(cm => cm.CarBrandId == brandId)
            .ToListAsync(cancellationToken);
    }

    public async Task<CarModel?> GetByBrandAndModelNameAsync(Guid brandId, string modelName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CarModel>()
#pragma warning disable CA1311
#pragma warning disable CA1304
#pragma warning disable CA1862
            .Where(cm => cm.CarBrandId == brandId && cm.ModelName.ToLower() == modelName.ToLower())
#pragma warning restore CA1862
#pragma warning restore CA1304
#pragma warning restore CA1311
            .FirstOrDefaultAsync(cancellationToken);
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
