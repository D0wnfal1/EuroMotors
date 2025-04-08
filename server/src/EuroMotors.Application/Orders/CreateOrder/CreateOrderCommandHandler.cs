using System.Data.Common;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    IUserRepository userRepository,
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    CartService cartService,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await using DbTransaction transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        User? user = null;
        if (request.UserId is not null && request.UserId != Guid.Empty)
        {
            user = await userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);

            if (user is null)
            {
                return Result.Failure<Guid>(UserErrors.NotFound(request.UserId.Value));
            }
        }

        var order = Order.Create(user?.Id, request.BuyerName, request.BuyerPhoneNumber, request.BuyerEmail, request.DeliveryMethod, request.ShippingAddress, request.PaymentMethod);

        Cart cart = await cartService.GetAsync(request.CartId, cancellationToken);

        if (!cart.CartItems.Any())
        {
            return Result.Failure<Guid>(CartErrors.Empty);
        }

        foreach (CartItem cartItem in cart.CartItems)
        {
            // This acquires a pessimistic lock or throws an exception if already locked.
            Product? product = await productRepository.GetWithLockAsync(
                cartItem.ProductId,
                cancellationToken);

            if (product is null)
            {
                return Result.Failure<Guid>(ProductErrors.NotFound(cartItem.ProductId));
            }

            product.SubtractProductQuantity(cartItem.Quantity);

            order.AddItem(product, cartItem.Quantity, cartItem.UnitPrice);
        }

        orderRepository.Insert(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        await cartService.ClearAsync(request.CartId, cancellationToken);

        return Result.Success(order.Id);
    }
}
