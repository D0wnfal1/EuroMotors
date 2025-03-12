using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Users.Update;

public sealed record UpdateUserInformationCommand(string Email, string FirstName, string LastName, string PhoneNumber, string City)
    : ICommand;
