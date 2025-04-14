using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Categories;

public sealed class SetCategoryAvailabilityRequest
{
    [JsonRequired]
    public bool IsAvailable { get; set; }
}
