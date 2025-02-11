using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.CreateCarModel;

public sealed record CreateCarModelCommand(string Brand, string Model) : ICommand<Guid>;
