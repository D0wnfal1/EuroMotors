using EuroMotors.Domain.CarModels;

namespace EuroMotors.Api.Controllers.CarModels;

public sealed class UpdateCarModelRequest
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int? StartYear { get; set; }
    public BodyType? BodyType { get; set; }
    public float? VolumeLiters { get; set; }
    public FuelType? FuelType { get; set; }
    public IFormFile? ImagePath { get; set; }
}
