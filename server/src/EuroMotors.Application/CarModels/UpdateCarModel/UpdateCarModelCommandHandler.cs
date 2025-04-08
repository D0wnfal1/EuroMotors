using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

internal sealed class UpdateCarModelCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCarModelCommand>
{
    public async Task<Result> Handle(UpdateCarModelCommand request, CancellationToken cancellationToken)
    {
        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(CarModelErrors.NotFound(request.CarModelId));
        }

        carModel.Update(request.Brand, request.Model, request.StartYear, request.EndYear, request.BodyType);

        carModel.UpdateEngineSpec(request.VolumeLiters, request.FuelType, request.HorsePower);

        if (request.Image is not null && request.Image.Length > 0)
        {
            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "carModels");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            if (!string.IsNullOrEmpty(carModel.ImagePath))
            {
                string oldImagePath = Path.Combine(projectRoot, "wwwroot", carModel.ImagePath.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
            string newFilePath = Path.Combine(basePath, fileName);

            await using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream, cancellationToken);
            }

            string newImageUrl = $"/images/carModels/{fileName}";
            carModel.SetImagePath(newImageUrl);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
