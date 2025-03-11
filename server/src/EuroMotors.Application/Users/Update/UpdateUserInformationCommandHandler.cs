using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Application.Users.Update;

internal sealed class UpdateUserInformationCommandHandler(IApplicationDbContext context, IUserRepository userRepository)
    : ICommandHandler<UpdateUserInformationCommand>
{
    public async Task<Result> Handle(UpdateUserInformationCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFoundByEmail);
        }

        user.UpdateContactInfo(command.PhoneNumber, command.City);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        userRepository.Update(user);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
