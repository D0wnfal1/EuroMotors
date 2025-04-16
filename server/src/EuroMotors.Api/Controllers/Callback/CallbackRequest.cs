using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Callback;

public class CallbackRequest
{
    [JsonRequired]
    public string Name { get; set; }
    [JsonRequired]
    public string Phone { get; set; }
}
