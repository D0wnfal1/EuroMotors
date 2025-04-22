using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.RemoveCarModelFromProduct;

internal sealed class RemoveCarModelFromProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveCarModelFromProductCommand>
{
    public async Task<Result> Handle(RemoveCarModelFromProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        product.RemoveCarModel(request.CarModelId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
} 