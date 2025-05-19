using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Delivery.GetWarehouse;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Delivery;
[Route("api/deliveries")]
[ApiController]
public class DeliveryController : ControllerBase
{
    [HttpPost("warehouses")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWarehouses([FromBody] GetWareHousesRequest request, IQueryHandler<GetWarehousesQuery, WarehousesResponse<Warehouse>> handler, CancellationToken cancellationToken)
    {
        var query = new GetWarehousesQuery(request.City, request.Query);

        Result<WarehousesResponse<Warehouse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}
