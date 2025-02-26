using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.Categories.DeleteImage;

internal sealed class DeleteCategoryImageCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryImageCommand>
{
    public async Task<Result> Handle(DeleteCategoryImageCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.Id));
        }

        category.DeleteImage();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
