using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class CreateCarModelCommandHandler(ICarModelRepository carModelRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateCarModelCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCarModelCommand request, CancellationToken cancellationToken)
    {
        var carModel = CarModel.Create(request.Brand, request.Model, request.StartYear, request.EndYear, request.BodyType, request.EngineSpec);

        if (request.Image != null && request.Image.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";

            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "CarModels");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string filePath = Path.Combine(basePath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream, cancellationToken);
            }

            string imageUrl = $"/images/CarModels/{fileName}";
            carModel.SetImagePath(imageUrl);
        }

        carModelRepository.Insert(carModel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return carModel.Id;
    }
}

