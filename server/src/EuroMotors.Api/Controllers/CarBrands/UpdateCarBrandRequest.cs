using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.CarBrands;

public sealed class UpdateCarBrandRequest
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    public IFormFile? Logo { get; set; }
}
