using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.UpdateProductImage;

internal sealed class UpdateProductImageCommandHandler(
    IProductImageRepository productImageRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateProductImageCommand>
{
    public async Task<Result> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return Result.Failure(ProductImageErrors.InvalidFile(request.File!));
        }

        ProductImage? existingImage = await productImageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingImage is null)
        {
            return Result.Failure(ProductImageErrors.ProductImageNotFound(request.Id));
        }

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";

        string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        string basePath = Path.Combine(projectRoot, "wwwroot", "images", "products");

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        string filePath = Path.Combine(basePath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        string imagePath = $"/images/products/{fileName}";

        existingImage.UpdateImage(imagePath, request.ProductId);
        productImageRepository.Update(existingImage);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
