using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.ProductImages;

public sealed class UploadProductImageRequest
{
    [JsonRequired]
    public IFormFile File { get; set; }

    [JsonRequired]
    public Guid ProductId { get; set; }
}
