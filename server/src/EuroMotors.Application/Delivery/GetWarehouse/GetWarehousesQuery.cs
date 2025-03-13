using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Delivery.GetWarehouse;

public sealed record GetWarehousesQuery(string City, string Query) : IQuery<WarehousesResponse<Warehouse>>;
