using EuroMotors.Application.Delivery.GetWarehouse;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Delivery;
[Route("api/deliveries")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly ISender _sender;

    public DeliveryController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost("warehouses")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWarehouses([FromBody] GetWareHousesRequest request)
    {
        var query = new GetWarehousesQuery(request.City, request.Query);

        Result<WarehousesResponse<Warehouse>> result = await _sender.Send(query);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}
