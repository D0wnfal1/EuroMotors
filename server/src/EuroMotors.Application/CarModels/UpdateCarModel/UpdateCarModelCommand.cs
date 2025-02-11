using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

public record UpdateCarModelCommand(Guid CarModelId, string Brand, string Model) : ICommand;
