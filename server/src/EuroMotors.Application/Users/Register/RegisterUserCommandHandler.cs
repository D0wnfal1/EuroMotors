using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using EuroMotors.Domain.Users.Events;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Application.Users.Register;

internal sealed class RegisterUserCommandHandler(IApplicationDbContext context, IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        var user = User.Create(command.Email, command.FirstName, command.LastName, passwordHasher.Hash(command.Password));

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        userRepository.Insert(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
