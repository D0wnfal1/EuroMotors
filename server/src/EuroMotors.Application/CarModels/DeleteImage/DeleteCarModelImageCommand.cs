using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.DeleteImage;

public sealed record DeleteCarModelImageCommand(Guid Id) : ICommand;
