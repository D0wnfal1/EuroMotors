using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
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

        return Result.Success();
    }
}
