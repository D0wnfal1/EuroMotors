using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class CreateCarModelCommandHandler(
    ICarModelRepository carModelRepository,
    ICarBrandRepository carBrandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCarModelCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCarModelCommand request, CancellationToken cancellationToken)
    {
        CarBrand? carBrand = await carBrandRepository.GetByIdAsync(request.CarBrandId, cancellationToken);

        if (carBrand is null)
        {
            return Result.Failure<Guid>(CarModelErrors.BrandNotFound(request.CarBrandId));
        }

        var carModel = CarModel.Create(
            carBrand,
            request.ModelName,
            request.StartYear,
            request.BodyType,
            request.EngineSpec);

        carModelRepository.Insert(carModel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return carModel.Id;
    }
}

