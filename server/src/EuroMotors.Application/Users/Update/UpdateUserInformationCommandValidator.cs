using FluentValidation;

namespace EuroMotors.Application.Users.Update;

internal sealed class UpdateUserInformationCommandValidator : AbstractValidator<UpdateUserInformationCommand>
{
    public UpdateUserInformationCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty();
        RuleFor(c => c.PhoneNumber).NotEmpty();
        RuleFor(c => c.City).NotEmpty();
    }
}
