using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository, 
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }

        if (!string.IsNullOrEmpty(category.ImagePath))
        {
            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "categories");
            string filePath = Path.Combine(basePath, Path.GetFileName(category.ImagePath));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        await categoryRepository.Delete(category.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await InvalidateCacheAsync(category.Id, cancellationToken);

        return Result.Success();
    }
    
    private async Task InvalidateCacheAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.Categories.GetById(categoryId), cancellationToken);
        
        await cacheService.RemoveAsync(CacheKeys.Categories.GetList(), cancellationToken);

        await cacheService.RemoveByPrefixAsync($"{CacheKeys.Categories.GetHierarchical()}", cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(categoryId), cancellationToken);
        
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
    }
}
