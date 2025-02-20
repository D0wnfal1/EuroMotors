using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Orders.ChangeOrderStatus;

internal sealed class ChangeOrderStatusCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork) : ICommandHandler<ChangeOrderStatusCommand>
{
    public async Task<Result> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        order.ChangeStatus(request.Status);

        if (order.Status == OrderStatus.Shipped)
        {
            foreach (OrderItem orderItem in order.OrderItems)
            {
                Product? product = await productRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);

                if (product is null)
                {
                    return Result.Failure(ProductErrors.NotFound(orderItem.ProductId));
                }

                Result result = product.SubtractProductQuantity(orderItem.Quantity);

                if (result.IsFailure)
                {
                    return result;
                }
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
