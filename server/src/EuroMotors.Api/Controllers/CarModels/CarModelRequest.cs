namespace EuroMotors.Api.Controllers.CarModels;

public sealed record CarModelRequest(string Brand, string Model, IFormFile? Image);
