using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.DeleteCarModel;

internal sealed class DeleteCarModelCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteCarModelCommand>
{
    public async Task<Result> Handle(DeleteCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.ModelNotFound(request.CarModelId));
        }

        await carModelRepository.Delete(carModel.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
