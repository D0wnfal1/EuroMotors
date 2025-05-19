using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Abstractions.Behaviors;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}
