using EuroMotors.Domain.Users.Events;
using MediatR;

namespace EuroMotors.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
