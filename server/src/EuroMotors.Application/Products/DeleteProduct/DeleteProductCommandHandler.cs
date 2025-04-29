using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IProductRepository productRepository, 
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }
        
        Guid categoryId = product.CategoryId;

        await productRepository.Delete(product.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(product.Id, categoryId, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.Products.GetById(productId), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(categoryId), cancellationToken);
    }
}
