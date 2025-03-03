using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.UpdateImage;

internal sealed class UpdateCategoryImageCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryImageCommand>
{
    public async Task<Result> Handle(UpdateCategoryImageCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.Id));
        }

        category.UpdateImage(request.Url);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
