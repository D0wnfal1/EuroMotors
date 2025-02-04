using EuroMotors.SharedKernel;

namespace EuroMotors.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
