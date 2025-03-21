using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.UploadProductImage;

internal sealed class UploadProductImageCommandHandler(
    IProductImageRepository productImageRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UploadProductImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return Result.Failure<Guid>(ProductImageErrors.InvalidFile(request.File!));
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

        string imageUrl = $"/images/products/{fileName}";

        var productImage = ProductImage.Create(imageUrl, request.ProductId);
        productImageRepository.Insert(productImage);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return productImage.Id;
    }
}
