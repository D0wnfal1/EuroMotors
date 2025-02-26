using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.DeleteImage;

internal sealed class DeleteCarModelImageCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteCarModelImageCommand>
{
    public async Task<Result> Handle(DeleteCarModelImageCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.Id, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.NotFound(request.Id));
        }

        carModel.DeleteImage();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
