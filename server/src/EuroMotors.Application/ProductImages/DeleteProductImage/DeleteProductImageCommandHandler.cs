using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.DeleteProductImage;

internal sealed class DeleteProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductImageCommand>
{
    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        ProductImage? productImage = await productImageRepository.GetByIdAsync(request.Id, cancellationToken);

        if (productImage is null)
        {
            return Result.Failure(ProductImageErrors.ProductImageNotFound(request.Id));
        }

        string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        string basePath = Path.Combine(projectRoot, "wwwroot", "images", "products");
        string filePath = Path.Combine(basePath, Path.GetFileName(productImage.Path));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        await productImageRepository.Delete(productImage.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
