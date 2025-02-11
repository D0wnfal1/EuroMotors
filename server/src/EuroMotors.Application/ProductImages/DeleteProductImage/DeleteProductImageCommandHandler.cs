using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.DeleteProductImage;

public class DeleteProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteProductImageCommand>
{
    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        ProductImage? productImage = await productImageRepository.GetByIdAsync(request.Id, cancellationToken);

        if (productImage is null)
        {
            return Result.Failure(ProductImageErrors.ProductImageNotFound(request.Id));
        }

        productImageRepository.Delete(productImage.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
