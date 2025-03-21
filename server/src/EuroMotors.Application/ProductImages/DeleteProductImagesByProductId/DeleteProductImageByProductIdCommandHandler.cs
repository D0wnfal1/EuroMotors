using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.DeleteProductImagesByProductId;

internal sealed class DeleteProductImageByProductIdCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductImageByProductIdCommand>
{
    public async Task<Result> Handle(DeleteProductImageByProductIdCommand request, CancellationToken cancellationToken)
    {
        List<ProductImage> images = await productImageRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (images is null || !images.Any())
        {
            return Result.Failure(ProductImageErrors.ProductImagesNotFound(request.ProductId));
        }

        foreach (ProductImage image in images)
        {
            string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
            string basePath = Path.Combine(projectRoot, "wwwroot", "images", "products");
            string filePath = Path.Combine(basePath, Path.GetFileName(image.Url));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await productImageRepository.Delete(image.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
