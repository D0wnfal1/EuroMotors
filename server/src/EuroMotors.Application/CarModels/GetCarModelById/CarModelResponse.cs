namespace EuroMotors.Application.CarModels.GetCarModelById;

public sealed class CarModelResponse
{
    public Guid Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public string BodyType { get; set; }
    public float VolumeLiters { get; set; }
    public string FuelType { get; set; }
    public string Slug { get; set; }
    public string? ImagePath { get; set; }
    public CarModelResponse() { }
}
