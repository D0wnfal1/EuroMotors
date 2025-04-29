using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;

namespace EuroMotors.Application.CarBrands.CreateCarBrand;

internal sealed class CreateCarBrandCommandHandler(
    ICarBrandRepository carBrandRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCarBrandCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCarBrandCommand request, CancellationToken cancellationToken)
    {
        var carBrand = CarBrand.Create(request.Name);

        if (request.Logo is not null && request.Logo.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Logo.FileName)}";

            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "CarBrands");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string filePath = Path.Combine(basePath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Logo.CopyToAsync(stream, cancellationToken);
            }

            string logoUrl = $"/images/CarBrands/{fileName}";
            carBrand.SetLogoPath(logoUrl);
        }

        carBrandRepository.Insert(carBrand);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(cancellationToken);

        return carBrand.Id;
    }
    
    private async Task InvalidateCacheAsync(CancellationToken cancellationToken)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarBrands.GetAllPrefix(), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.CarBrands.GetAllForModels(), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetSelection(), cancellationToken);
    }
}
