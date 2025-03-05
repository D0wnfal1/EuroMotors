using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.UnitTests.Infrastructure;

public abstract class BaseTest
{
    public static T AssertDomainEventWasPublished<T>(Entity entity)
        where T : IDomainEvent
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        return domainEvent is null ? throw new Exception($"{typeof(T).Name} was not published") : domainEvent;
    }
}
