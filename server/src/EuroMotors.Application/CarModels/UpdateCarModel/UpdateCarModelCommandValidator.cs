using FluentValidation;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

internal sealed class UpdateCarModelCommandValidator : AbstractValidator<UpdateCarModelCommand>
{
    public UpdateCarModelCommandValidator()
    {
        RuleFor(c => c.CarModelId).NotEmpty();

        RuleFor(c => c.Brand).NotEmpty();

        RuleFor(c => c.Model).NotEmpty();

        RuleFor(c => c.StartYear)
            .GreaterThanOrEqualTo(1900)
            .LessThanOrEqualTo(DateTime.Now.Year);

        RuleFor(c => c.EndYear)
            .GreaterThanOrEqualTo(c => c.StartYear).When(c => c.EndYear.HasValue)
            .LessThanOrEqualTo(DateTime.Now.Year).When(c => c.EndYear.HasValue);

        RuleFor(c => c.BodyType)
            .IsInEnum();

        RuleFor(c => c.VolumeLiters)
            .GreaterThan(0)
            .LessThanOrEqualTo(10)
            .When(c => c.VolumeLiters.HasValue);

        RuleFor(c => c.FuelType)
            .IsInEnum()
            .When(c => c.FuelType.HasValue);

        RuleFor(c => c.HorsePower)
            .GreaterThan(0)
            .LessThanOrEqualTo(2000)
            .When(c => c.HorsePower.HasValue);
    }
}
