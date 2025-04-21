namespace EuroMotors.Application.CarBrands.GetCarBrands;

public sealed class CarBrandResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? LogoPath { get; set; }

    public CarBrandResponse() { }
}