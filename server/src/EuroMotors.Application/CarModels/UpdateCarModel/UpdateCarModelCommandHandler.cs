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
        
        var brandId = carModel.CarBrandId;

        carModel.Update(request.ModelName, request.StartYear, request.BodyType);

        if (request.EngineVolumeLiters.HasValue || request.EngineFuelType.HasValue)
        {
            carModel.UpdateEngineSpec(request.EngineVolumeLiters, request.EngineFuelType);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Инвалидируем кеш после обновления модели
        await InvalidateCacheAsync(request.Id, brandId, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Guid modelId, Guid brandId, CancellationToken cancellationToken)
    {
        // Инвалидируем кеш конкретной модели
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetById(modelId), cancellationToken);
        
        // Инвалидируем список моделей
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetAllPrefix(), cancellationToken);
        
        // Инвалидируем список моделей для конкретного бренда
        await cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetByBrandId(brandId), cancellationToken);
        
        // Инвалидируем выборку моделей
        await cacheService.RemoveAsync(CacheKeys.CarModels.GetSelection(), cancellationToken);
        
        // Инвалидируем связанные с моделями продукты
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
    }
}
