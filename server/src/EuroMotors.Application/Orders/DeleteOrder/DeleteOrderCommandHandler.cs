using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteOrderCommand>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        await orderRepository.Delete(order.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
