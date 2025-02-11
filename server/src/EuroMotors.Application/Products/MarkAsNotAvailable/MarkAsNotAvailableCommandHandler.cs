using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.MarkAsNotAvailable;

internal sealed class MarkAsNotAvailableCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<MarkAsNotAvailableCommand>
{
    public async Task<Result> Handle(MarkAsNotAvailableCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        product.MarkAsNotAvailable();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

