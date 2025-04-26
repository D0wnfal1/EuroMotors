using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.CopyProduct;

internal sealed class CopyProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CopyProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CopyProductCommand request, CancellationToken cancellationToken)
    {
        Product? sourceProduct = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (sourceProduct is null)
        {
            return Result.Failure<Guid>(ProductErrors.NotFound(request.ProductId));
        }

        var copiedProduct = Product.Create(
            $"{sourceProduct.Name} (Copy)",
            sourceProduct.Specifications.Select(s => (s.SpecificationName, s.SpecificationValue)),
            sourceProduct.VendorCode,
            sourceProduct.CategoryId,
            sourceProduct.CarModels,
            sourceProduct.Price,
            sourceProduct.Discount,
            0);

        productRepository.Insert(copiedProduct);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return copiedProduct.Id;
    }
} 
