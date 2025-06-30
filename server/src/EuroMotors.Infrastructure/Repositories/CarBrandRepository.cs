using EuroMotors.Domain.CarBrands;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CarBrandRepository : Repository<CarBrand>, ICarBrandRepository
{
    public CarBrandRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<CarBrand> GetAll()
    {
        return _dbContext.Set<CarBrand>();
    }

    public async Task<CarBrand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CarBrand>()
#pragma warning disable CA1311
#pragma warning disable CA1304
#pragma warning disable CA1304
#pragma warning disable CA1862
            .FirstOrDefaultAsync(cb => cb.Name.ToLower() == name.ToLower(), cancellationToken);
#pragma warning restore CA1862
#pragma warning restore CA1304
#pragma warning restore CA1304
#pragma warning restore CA1311
    }

    public async Task Delete(Guid carBrandId)
    {
        CarBrand? carBrand = await GetByIdAsync(carBrandId);

        if (carBrand is null)
        {
            return;
        }

        _dbContext.Set<CarBrand>().Remove(carBrand);

        await _dbContext.SaveChangesAsync();
    }
}
