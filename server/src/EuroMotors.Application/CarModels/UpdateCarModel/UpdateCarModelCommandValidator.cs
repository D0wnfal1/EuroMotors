using FluentValidation;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

internal sealed class UpdateCarModelCommandValidator : AbstractValidator<UpdateCarModelCommand>
{
    public UpdateCarModelCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();

        RuleFor(c => c.ModelName).NotEmpty();

        RuleFor(c => c.StartYear)
            .GreaterThanOrEqualTo(1900)
            .LessThanOrEqualTo(DateTime.Now.Year);

        RuleFor(c => c.BodyType)
            .IsInEnum();

        RuleFor(c => c.EngineVolumeLiters)
            .GreaterThan(0)
            .LessThanOrEqualTo(10)
            .When(c => c.EngineVolumeLiters.HasValue);

        RuleFor(c => c.EngineFuelType)
            .IsInEnum()
            .When(c => c.EngineFuelType.HasValue);
    }
}
