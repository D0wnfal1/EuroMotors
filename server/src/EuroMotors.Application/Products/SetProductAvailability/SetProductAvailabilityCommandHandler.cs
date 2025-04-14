using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.SetProductAvailability;

internal sealed class SetProductAvailabilityCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<SetProductAvailabilityCommand>
{
    public async Task<Result> Handle(SetProductAvailabilityCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        product.SetAvailability(request.IsAvailable);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

