using System.Net.Sockets;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(IUserRepository userRepository, IProductRepository productRepository, CartService cartService)
    : ICommandHandler<RemoveItemFromCartCommand>
{
    public async Task<Result> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        await cartService.RemoveItemAsync(user.Id, product.Id, cancellationToken);

        return Result.Success();
    }
}
