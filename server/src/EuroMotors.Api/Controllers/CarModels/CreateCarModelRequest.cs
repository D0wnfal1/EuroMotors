using EuroMotors.Domain.CarModels;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.CarModels;

public sealed class CreateCarModelRequest
{
    [JsonRequired]
    public Guid CarBrandId { get; set; }

    [JsonRequired]
    public string ModelName { get; set; }

    [JsonRequired]
    public int StartYear { get; set; }

    [JsonRequired]
    public BodyType BodyType { get; set; }

    [JsonRequired]
    public float VolumeLiters { get; set; }

    [JsonRequired]
    public FuelType FuelType { get; set; }

}
