using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.DeleteCarModel;

internal sealed class DeleteCarModelCommandHandler(
    ICarModelRepository carModelRepository, 
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCarModelCommand>
{
    public async Task<Result> Handle(DeleteCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.ModelNotFound(request.CarModelId));
        }
        
        Guid brandId = carModel.CarBrandId;

        await carModelRepository.Delete(carModel.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(request.CarModelId, brandId, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Guid modelId, Guid brandId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetById(modelId), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetAllPrefix(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetByBrandId(brandId), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetSelection(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
    }
}
