using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarBrands.UpdateCarBrand;

internal sealed class UpdateCarBrandCommandHandler(
    ICarBrandRepository carBrandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarBrandCommand>
{
    public async Task<Result> Handle(UpdateCarBrandCommand request, CancellationToken cancellationToken)
    {
        CarBrand? carBrand = await carBrandRepository.GetByIdAsync(request.CarBrandId, cancellationToken);

        if (carBrand is null)
        {
            return Result.Failure(CarModelErrors.BrandNotFound(request.CarBrandId));
        }

        carBrand.Update(request.Name);

        if (request.Logo is not null && request.Logo.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Logo.FileName)}";

            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "CarBrands");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string filePath = Path.Combine(basePath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Logo.CopyToAsync(stream, cancellationToken);
            }

            string logoUrl = $"/images/CarBrands/{fileName}";
            Result setLogoResult = carBrand.SetLogoPath(logoUrl);

            if (setLogoResult.IsFailure)
            {
                return setLogoResult;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
