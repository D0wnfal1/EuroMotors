using EuroMotors.Application.Abstractions.Behaviors;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
