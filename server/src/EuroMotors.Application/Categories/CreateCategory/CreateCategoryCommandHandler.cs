using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId != null)
        {
            Category? parentCategory = await categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);

            if (parentCategory?.ParentCategoryId != null)
            {
                return Result.Failure<Guid>(new Error("ParentCategoryError", "The parent category cannot be a subcategory.", ErrorType.Conflict));
            }
        }

        var category = Category.Create(request.Name, request.ParentCategoryId);

        if (request.SubcategoryNames != null)
        {
            foreach (string subcategoryName in request.SubcategoryNames)
            {
                var subcategory = Category.Create(subcategoryName, category.Id);
                category.AddSubcategory(subcategory);
            }
        }

        if (request.Image is { Length: > 0 })
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";

            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "categories");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string filePath = Path.Combine(basePath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream, cancellationToken);
            }

            string imageUrl = $"/images/categories/{fileName}";
            category.SetImagePath(imageUrl);
        }

        categoryRepository.Insert(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
