using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarBrands.DeleteCarBrand;

internal sealed class DeleteCarBrandCommandHandler(
    ICarBrandRepository carBrandRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCarBrandCommand>
{
    public async Task<Result> Handle(DeleteCarBrandCommand request, CancellationToken cancellationToken)
    {
        CarBrand? carBrand = await carBrandRepository.GetByIdAsync(request.CarBrandId, cancellationToken);

        if (carBrand is null)
        {
            return Result.Failure(CarModelErrors.BrandNotFound(request.CarBrandId));
        }

        await carBrandRepository.Delete(request.CarBrandId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(request.CarBrandId, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Guid brandId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.CarBrands.GetById(brandId), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarBrands.GetAllPrefix(), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.CarBrands.GetAllForModels(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetByBrandId(brandId), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetSelection(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetAllPrefix(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
    }
}
