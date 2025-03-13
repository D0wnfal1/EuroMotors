namespace EuroMotors.Application.Delivery.GetWarehouse;

public sealed class WarehousesResponse<T>
{
    public bool Success { get; set; }
    public List<T> Data { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}


