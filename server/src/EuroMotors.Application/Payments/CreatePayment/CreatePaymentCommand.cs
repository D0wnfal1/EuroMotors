using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(Guid OrderId) : ICommand<Dictionary<string, string>>;
