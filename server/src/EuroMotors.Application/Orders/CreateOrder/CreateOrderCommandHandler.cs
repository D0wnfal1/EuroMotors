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
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await using DbTransaction transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        User? User = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (User is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        var order = Order.Create(User);

        Cart cart = await cartService.GetAsync(User.Id, cancellationToken);

        if (!cart.CartItems.Any())
        {
            return Result.Failure(CartErrors.Empty);
        }

        foreach (CartItem cartItem in cart.CartItems)
        {
            // This acquires a pessimistic lock or throws an exception if already locked.
            Product? product = await productRepository.GetWithLockAsync(
                cartItem.ProductId,
                cancellationToken);

            if (product is null)
            {
                return Result.Failure(ProductErrors.NotFound(cartItem.ProductId));
            }

            order.AddItem(product, cartItem.Quantity, cartItem.UnitPrice);
        }

        orderRepository.Insert(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        await cartService.ClearAsync(User.Id, cancellationToken);

        return Result.Success();
    }
}
