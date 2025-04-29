using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

internal sealed class UpdateCarModelCommandHandler(
    ICarModelRepository carModelRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarModelCommand>
{
    public async Task<Result> Handle(UpdateCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.Id, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.ModelNotFound(request.Id));
        }
        
        Guid brandId = carModel.CarBrandId;

        carModel.Update(request.ModelName, request.StartYear, request.BodyType);

        if (request.EngineVolumeLiters.HasValue || request.EngineFuelType.HasValue)
        {
            carModel.UpdateEngineSpec(request.EngineVolumeLiters, request.EngineFuelType);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(request.Id, brandId, cancellationToken);

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
