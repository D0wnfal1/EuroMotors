using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateImage;

internal sealed class UpdateCarModelImageCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarModelImageCommand>
{
    public async Task<Result> Handle(UpdateCarModelImageCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.Id, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.NotFound(request.Id));
        }

        carModel.UpdateImage(request.Url);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
