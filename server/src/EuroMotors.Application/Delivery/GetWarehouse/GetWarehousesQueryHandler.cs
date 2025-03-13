using EuroMotors.Application.Abstractions.Delivery;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Delivery.GetWarehouse;

internal sealed class GetWarehousesQueryHandler(IDeliveryService deliveryService) : IQueryHandler<GetWarehousesQuery, WarehousesResponse<Warehouse>>
{
    public async Task<Result<WarehousesResponse<Warehouse>>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        List<Warehouse> warehouseList = await deliveryService.GetWarehousesAsync(request.City, request.Query);
        var warehouses = new WarehousesResponse<Warehouse>
        {
            Success = true,
            Data = warehouseList
        };
        return Result.Success<WarehousesResponse<Warehouse>>(warehouses);
    }
}
