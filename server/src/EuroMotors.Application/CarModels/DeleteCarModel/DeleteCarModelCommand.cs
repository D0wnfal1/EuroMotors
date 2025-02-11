using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.DeleteCarModel;

public sealed record DeleteCarModelCommand(Guid CarModelId) : ICommand;
