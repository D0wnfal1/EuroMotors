using EuroMotors.Domain.Users.Events;
using MediatR;

namespace EuroMotors.Application.Users.Update;

internal sealed class UpdatedUserInformationDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
