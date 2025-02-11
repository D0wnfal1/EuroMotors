using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductDiscount;

internal sealed class UpdateProductDiscountCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductDiscountCommand>
{
    public async Task<Result> Handle(UpdateProductDiscountCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        product.UpdateDiscount(request.Discount);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

