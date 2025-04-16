namespace EuroMotors.Application.CarModels.GetCarModelSelection;

public sealed class CarModelSelectionResponse
{
    public List<string> Brands { get; set; } = [];
    public List<string> Models { get; set; } = [];
    public List<int> Years { get; set; } = [];
    public List<string> BodyTypes { get; set; } = [];
    public List<string> EngineSpecs { get; set; } = [];

    public CarModelSelectionResponse()
    {
        
    }
}
