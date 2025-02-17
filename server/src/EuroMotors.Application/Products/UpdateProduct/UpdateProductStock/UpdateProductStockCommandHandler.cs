using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductStock;

internal sealed class UpdateProductStockCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductStockCommand>
{
    public async Task<Result> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        product.UpdateStock(request.Stock);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

