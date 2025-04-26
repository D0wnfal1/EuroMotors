using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProductCarModels;

internal sealed class UpdateProductCarModelsCommandHandler(
    IProductRepository productRepository,
    ICarModelRepository carModelRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCarModelsCommand>
{
    public async Task<Result> Handle(UpdateProductCarModelsCommand request, CancellationToken cancellationToken)
    {
        Product? product =
            await productRepository.GetByIdWithCarModelsAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        List<CarModel> carModels = [];

        foreach (Guid carModelId in request.CarModelIds)
        {
            CarModel? carModel = await carModelRepository.GetByIdAsync(carModelId, cancellationToken);

            if (carModel is null)
            {
                return Result.Failure(CarModelErrors.ModelNotFound(carModelId));
            }

            carModels.Add(carModel);
        }

        product.UpdateCarModels(carModels);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
