using EuroMotors.Domain.CarModels;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.CarModels;

public sealed class CreateCarModelRequest
{
    [JsonRequired]
    public string Brand { get; set; }
    [JsonRequired]
    public string Model { get; set; }
    [JsonRequired]
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    [JsonRequired]
    public BodyType BodyType { get; set; }
    [JsonRequired]
    public float EngineSpecVolumeLiters { get; set; }
    
    [JsonRequired]
    public FuelType EngineSpecFuelType { get; set; }

    [JsonRequired] public int EngineSpecHorsePower { get; set; }
    public IFormFile? ImagePath { get; set; }

}
