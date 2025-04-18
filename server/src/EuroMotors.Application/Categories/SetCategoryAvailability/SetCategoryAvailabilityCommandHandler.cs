using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.SetCategoryAvailability;

internal sealed class SetCategoryAvailabilityCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<SetCategoryAvailabilityCommand>
{
    public async Task<Result> Handle(SetCategoryAvailabilityCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }



        category.SetAvailability(request.IsAvailable);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
