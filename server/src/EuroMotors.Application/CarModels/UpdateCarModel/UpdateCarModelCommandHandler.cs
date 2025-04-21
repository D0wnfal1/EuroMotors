using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

internal sealed class UpdateCarModelCommandHandler(
    ICarModelRepository carModelRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarModelCommand>
{
    public async Task<Result> Handle(UpdateCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.Id, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.ModelNotFound(request.Id));
        }

        carModel.Update(request.ModelName, request.StartYear, request.BodyType);

        if (request.EngineVolumeLiters.HasValue || request.EngineFuelType.HasValue)
        {
            carModel.UpdateEngineSpec(request.EngineVolumeLiters, request.EngineFuelType);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
