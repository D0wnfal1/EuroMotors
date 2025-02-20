using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.UpdateProductImage;

internal sealed class UpdateProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductImageCommand>
{
    public async Task<Result> Handle(UpdateProductImageCommand request, CancellationToken cancellationToken)
    {
        ProductImage? productImage = await productImageRepository.GetByIdAsync(request.Id, cancellationToken);

        if (productImage is null)
        {
            return Result.Failure(ProductImageErrors.ProductImageNotFound(request.Id));
        }

        productImage.UpdateUrl(request.Url);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
