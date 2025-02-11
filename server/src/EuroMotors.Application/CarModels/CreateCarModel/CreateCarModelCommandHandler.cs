using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class CreateCarModelCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateCarModelCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCarModelCommand request, CancellationToken cancellationToken)
    {
        var carModel = CarModel.Create(request.Brand, request.Model);

        carModelRepository.Insert(carModel);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return carModel.Id;
    }
}
