using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarBrands.DeleteCarBrand;

public sealed record DeleteCarBrandCommand(Guid CarBrandId) : ICommand;