using EuroMotors.Domain.CarModels;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.CarModels;

public sealed class UpdateCarModelRequest
{
    [JsonRequired]
    public string Model { get; set; }

    [JsonRequired]
    public int StartYear { get; set; }

    [JsonRequired]
    public BodyType BodyType { get; set; }

    public float? VolumeLiters { get; set; }

    public FuelType? FuelType { get; set; }

    public IFormFile? ImagePath { get; set; }
}
