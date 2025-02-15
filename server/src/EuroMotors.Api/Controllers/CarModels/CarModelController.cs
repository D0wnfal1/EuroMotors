using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.CarModels;

[Route("api/car_models")]
[ApiController]
public class CarModelController : ControllerBase
{
    private readonly ISender _sender;

    public CarModelController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarModelById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarModelByIdQuery(id);

        Result<CarModelResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }
}

