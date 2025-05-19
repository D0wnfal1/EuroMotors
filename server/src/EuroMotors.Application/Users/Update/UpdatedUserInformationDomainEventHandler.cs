using EuroMotors.Application.Abstractions.Behaviors;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Application.Users.Update;

internal sealed class UpdatedUserInformationDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
