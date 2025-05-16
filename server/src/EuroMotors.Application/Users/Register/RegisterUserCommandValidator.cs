using FluentValidation;

namespace EuroMotors.Application.Users.Register;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Пароль повинен містити мінімум одну велику літеру")
            .Matches("[a-z]").WithMessage("Пароль повинен містити мінімум одну малу літеру")
            .Matches("[0-9]").WithMessage("Пароль повинен містити мінімум одну цифру")
            .Matches("[^a-zA-Z0-9]").WithMessage("Пароль повинен містити мінімум один спеціальний символ");
    }
}
