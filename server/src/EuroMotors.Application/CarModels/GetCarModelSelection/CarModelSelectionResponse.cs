namespace EuroMotors.Application.CarModels.GetCarModelSelection;

public sealed class CarModelSelectionResponse
{
    public List<Guid> Ids { get; set; } = [];
    public List<BrandInfo> Brands { get; set; } = [];
    public List<string> Models { get; set; } = [];
    public List<int> Years { get; set; } = [];
    public List<string> BodyTypes { get; set; } = [];
    public List<string> EngineSpecs { get; set; } = [];

    public CarModelSelectionResponse()
    {
    }
}

public sealed class BrandInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public BrandInfo() { }

    public BrandInfo(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
