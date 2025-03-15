using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid CartId, Guid? UserId, DeliveryMethod DeliveryMethod, string? ShippingAddress, PaymentMethod PaymentMethod) : ICommand<Guid>;
