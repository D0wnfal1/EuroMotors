using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

public record UpdateCarModelCommand(Guid CarModelId, string Brand, string Model, int? StartYear, BodyType? BodyType, float? VolumeLiters,
    FuelType? FuelType, IFormFile? Image) : ICommand;
