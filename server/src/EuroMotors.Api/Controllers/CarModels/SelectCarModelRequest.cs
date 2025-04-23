namespace EuroMotors.Api.Controllers.CarModels;

public sealed class SelectCarModelRequest
{
    public Guid? BrandId { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? StartYear { get; set; }
    public string? BodyType { get; set; }
}
