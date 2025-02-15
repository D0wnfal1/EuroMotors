using System.Data;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
	IUserRepository userRepository,
	IOrderRepository orderRepository,
	IProductRepository productRepository,
	IPaymentRepository paymentRepository,
	IPaymentService paymentService,
	ICartRepository cartRepository,
	IUnitOfWork unitOfWork,
	IDbConnectionFactory dbConnectionFactory) : ICommandHandler<CreateOrderCommand>
{
	public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
	{
		using IDbConnection connection = dbConnectionFactory.CreateConnection();

		User? user = await userRepository.GetByIdAsync(request.CustomerId, cancellationToken);

		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound(request.CustomerId));
		}

		var order = Order.Create(user.Id);

		Cart? cart = await cartRepository.GetByUserIdAsync(user.Id, cancellationToken);

		if (cart == null || !cart.CartItems.Any())
		{
			return Result.Failure(CartErrors.Empty);
		}

		foreach (CartItem cartItem in cart.CartItems)
		{
			// This acquires a pessimistic lock or throws an exception if already locked.
			Product? product = await productRepository.GetByIdAsync(
				cartItem.ProductId,
				cancellationToken);

			if (product is null)
			{
				return Result.Failure(ProductErrors.NotFound(cartItem.ProductId));
			}

			Result result = product.UpdateStock(cartItem.Quantity);

			if (result.IsFailure)
			{
				return Result.Failure(result.Error);
			}

			order.AddItem(product.Id, cartItem.Quantity, cartItem.UnitPrice);
		}

		orderRepository.Insert(order);

		// We're faking a payment gateway request here...
		PaymentResponse paymentResponse = await paymentService.ChargeAsync(order.TotalPrice);

		var payment = Payment.Create(
			order,
			paymentResponse.TransactionId,
			paymentResponse.Amount);

		paymentRepository.Insert(payment);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		cart.Clear();

        cartRepository.Update(cart);

        return Result.Success();
	}
}
