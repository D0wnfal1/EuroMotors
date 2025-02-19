using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.ChangeOrderStatus;

public class ChangeOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : ICommandHandler<ChangeOrderStatusCommand>
{
    public async Task<Result> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        order.ChangeStatus(request.Status);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
