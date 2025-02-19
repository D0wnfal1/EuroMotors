using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Orders.SetPayment;

public class SetPaymentCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : ICommandHandler<SetPaymentCommand>
{
    public async Task<Result> Handle(SetPaymentCommand request, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            return Result.Failure(OrderErrors.NotFound(request.OrderId));
        }

        order.SetPaymentId(request.PaymentId);

        order.ChangeStatus(request.PaymentStatus switch
        {
            PaymentStatus.Success => OrderStatus.Paid,
            PaymentStatus.Error => OrderStatus.Pending,
            PaymentStatus.Failure => OrderStatus.Pending,
            PaymentStatus.Reversed => OrderStatus.Refunded,
            _ => order.Status
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
