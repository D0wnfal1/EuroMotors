using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandHandler(
    IUserRepository customerRepository,
    IProductRepository ticketTypeRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        User? user = await customerRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        Product? product = await ticketTypeRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        if (product.Stock < request.Quantity)
        {
            return Result.Failure(ProductErrors.NotEnoughStock(product.Stock));
        }



        Cart cart = await cartRepository.GetByUserIdAsync(user.Id, cancellationToken) ?? Cart.Create(user.Id);

        var cartItem = CartItem.Create(product, cart.Id, request.Quantity);

        cart.AddItem(cartItem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
