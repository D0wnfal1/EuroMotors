namespace EuroMotors.Application.CarModels.GetCarModelById;

public sealed class CarModelResponse
{
    public Guid Id { get; set; }
    public Guid CarBrandId { get; set; }
    public string BrandName { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public int StartYear { get; set; }
    public string BodyType { get; set; } = null!;
    public float VolumeLiters { get; set; }
    public string FuelType { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? ImagePath { get; set; }
    public CarModelResponse() { }
}
