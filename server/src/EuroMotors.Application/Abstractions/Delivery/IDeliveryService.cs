using EuroMotors.Application.Delivery.GetWarehouse;

namespace EuroMotors.Application.Abstractions.Delivery;

public interface IDeliveryService
{
    Task<List<Warehouse>> GetWarehousesAsync(string cityName, string query);
}
