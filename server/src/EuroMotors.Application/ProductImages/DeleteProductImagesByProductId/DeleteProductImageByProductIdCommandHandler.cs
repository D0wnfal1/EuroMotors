using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.DeleteProductImagesByProductId;

internal sealed class DeleteProductImageByProductIdCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteProductImageByProductIdCommand>
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
            await productImageRepository.Delete(image.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
