using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.CreateProductImage;

internal sealed class CreateProductImageCommandHandler(IProductImageRepository productImageRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateProductImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
    {
        var productImage = ProductImage.Create(request.Url, request.ProductId);

        productImageRepository.Insert(productImage);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return productImage.Id;
    }
}
