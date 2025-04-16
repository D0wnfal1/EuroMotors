using FluentValidation;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class CreateCarModelCommandValidator : AbstractValidator<CreateCarModelCommand>
{
    public CreateCarModelCommandValidator()
    {
        RuleFor(c => c.Brand).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Model).MaximumLength(100).NotEmpty();
        RuleFor(c => c.StartYear)
            .GreaterThanOrEqualTo(1900)
            .LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(c => c.BodyType)
            .IsInEnum();
        RuleFor(c => c.EngineSpec)
            .NotNull();
    }
}
