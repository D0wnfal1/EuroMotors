using System.Text.Json.Serialization;

namespace EuroMotors.Api.Controllers.Delivery;

public class GetWareHousesRequest
{
    [JsonRequired]
    public string City { get; set; }
    [JsonRequired]
    public string Query { get; set; }
}
