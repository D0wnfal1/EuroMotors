using EuroMotors.Domain.Users.Events;
using MediatR;

namespace EuroMotors.Application.Users.Update;

internal sealed class UpdatedUserInformationDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send an email verification link, etc.
        return Task.CompletedTask;
    }
}
