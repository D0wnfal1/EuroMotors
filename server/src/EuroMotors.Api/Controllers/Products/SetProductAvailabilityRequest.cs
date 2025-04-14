using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Products;

public sealed class SetCategoryAvailabilityRequest
{
    [JsonRequired]
    public bool IsAvailable { get; set; }
}
