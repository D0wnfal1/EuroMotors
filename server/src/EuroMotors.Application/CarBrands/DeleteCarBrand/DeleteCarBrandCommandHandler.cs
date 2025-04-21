using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarBrands.DeleteCarBrand;

internal sealed class DeleteCarBrandCommandHandler(
    ICarBrandRepository carBrandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCarBrandCommand>
{
    public async Task<Result> Handle(DeleteCarBrandCommand request, CancellationToken cancellationToken)
    {
        CarBrand? carBrand = await carBrandRepository.GetByIdAsync(request.CarBrandId, cancellationToken);

        if (carBrand is null)
        {
            return Result.Failure(CarModelErrors.BrandNotFound(request.CarBrandId));
        }

        await carBrandRepository.Delete(request.CarBrandId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
