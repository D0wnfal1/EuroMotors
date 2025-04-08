using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }

        category.ChangeName(request.Name);

        if (request.ParentCategoryId.HasValue)
        {
            Category parentCategory = await categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory is null)
            {
                return Result.Failure(CategoryErrors.NotFound(request.ParentCategoryId.Value));
            }
            category.SetParent(parentCategory);
        }

        if (request.Image is not null && request.Image.Length > 0)
        {
            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "categories");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                string oldImagePath = Path.Combine(projectRoot, "wwwroot", category.ImagePath.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
            string newFilePath = Path.Combine(basePath, fileName);

            await using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream, cancellationToken);
            }

            string newImageUrl = $"/images/categories/{fileName}";
            category.SetImagePath(newImageUrl);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
