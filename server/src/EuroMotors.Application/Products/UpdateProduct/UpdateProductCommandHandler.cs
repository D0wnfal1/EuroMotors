using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }
        
        // Сохраняем старый идентификатор категории для возможной инвалидации кеша
        Guid oldCategoryId = product.CategoryId;

        IEnumerable<(string SpecificationName, string SpecificationValue)> specs = request.Specifications
            .Select(s => (s.SpecificationName, s.SpecificationValue));

        product.Update(
            request.Name,
            specs,
            request.VendorCode,
            request.CategoryId,
            product.CarModels,
            request.Price,
            request.Discount,
            request.Stock);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Инвалидируем кеш после обновления
        await InvalidateCacheAsync(product, oldCategoryId, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Product product, Guid oldCategoryId, CancellationToken cancellationToken)
    {
        // Инвалидируем кеш конкретного продукта
        await cacheService.RemoveAsync(CacheKeys.Products.GetById(product.Id), cancellationToken);
        
        // Инвалидируем списки продуктов
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
        
        // Если категория изменилась, инвалидируем кеш для старой и новой категорий
        if (oldCategoryId != product.CategoryId)
        {
            await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(oldCategoryId), cancellationToken);
        }
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(product.CategoryId), cancellationToken);
    }
}
