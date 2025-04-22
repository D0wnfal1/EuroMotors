using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.AddCarModelToProduct;

internal sealed class AddCarModelToProductCommandHandler(
    IProductRepository productRepository,
    ICarModelRepository carModelRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddCarModelToProductCommand>
{
    public async Task<Result> Handle(AddCarModelToProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.ModelNotFound(request.CarModelId));
        }

        product.AddCarModel(carModel);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
} 