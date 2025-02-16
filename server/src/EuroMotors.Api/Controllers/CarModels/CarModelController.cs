using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.CarModels.UpdateCarModel;
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

    [HttpGet]
    public async Task<IActionResult> GetCarModels(CancellationToken cancellationToken)
    {
        var query = new GetCarModelsQuery();

        Result<IReadOnlyCollection<CarModelResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarModelById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarModelByIdQuery(id);

        Result<CarModelResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCarModel([FromBody] CreateCarModelCommand command, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCarModelById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCarModel(Guid id, [FromBody] UpdateCarModelCommand command, CancellationToken cancellationToken)
    {
        if (id != command.CarModelId)
        {
            return BadRequest("ID mismatch");
        }

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCarModel(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCarModelCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

