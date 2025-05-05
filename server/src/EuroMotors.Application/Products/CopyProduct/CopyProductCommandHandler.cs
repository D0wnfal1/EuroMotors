using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.CopyProduct;

internal sealed class CopyProductCommandHandler(
    IProductRepository productRepository,
    ICacheService cacheService,
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

        await InvalidateCacheAsync(copiedProduct, cancellationToken);

        return copiedProduct.Id;
    }

    private async Task InvalidateCacheAsync(Product product, CancellationToken cancellationToken)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);

        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(product.CategoryId), cancellationToken);

        foreach (CarModel carModel in product.CarModels)
        {
            await cacheService.RemoveByPrefixAsync($"{CacheKeys.Products.GetAllPrefix()}:brand:", cancellationToken);
        }
    }
}
