using FluentValidation;

namespace EuroMotors.Application.CarModels.UpdateImage;

internal sealed class UpdateCarModelImageCommandValidator : AbstractValidator<UpdateCarModelImageCommand>
{
    public UpdateCarModelImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Url).NotEmpty();
    }
}
