using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.CarBrands;

public sealed class CreateCarBrandRequest
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    public IFormFile? Logo { get; set; }
}
