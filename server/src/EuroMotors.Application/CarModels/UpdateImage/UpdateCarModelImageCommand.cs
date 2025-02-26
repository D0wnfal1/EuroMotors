using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.UpdateImage;

public sealed record UpdateCarModelImageCommand(Guid Id, Uri Url) : ICommand;
