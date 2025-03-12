using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(Guid OrderId) : ICommand<Dictionary<string, string>>;
