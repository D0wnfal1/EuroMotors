using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.CreatePayment;

internal sealed class CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork, IPaymentService paymentService) : ICommandHandler<CreatePaymentCommand>
{
    public async Task<Result> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<string>(OrderErrors.NotFound(request.OrderId));
        }

        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Pending, order.TotalPrice);

        paymentRepository.Insert(payment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        string paymentUrl = await paymentService.CreatePaymentAsync(payment);

        return Result.Success(paymentUrl);
    }
}
