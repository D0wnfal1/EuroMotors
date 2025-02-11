using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

public sealed class UpdateCarModelCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarModelCommand>
{
    public async Task<Result> Handle(UpdateCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.NotFound(request.CarModelId));
        }

        carModel.Update(request.Brand, request.Model);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
