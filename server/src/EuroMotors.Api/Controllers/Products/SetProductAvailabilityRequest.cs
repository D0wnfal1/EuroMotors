using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Products;

public sealed class SetProductAvailabilityRequest
{
    [JsonRequired]
    public bool IsAvailable { get; set; }
}
