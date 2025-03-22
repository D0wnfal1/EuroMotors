using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.DeleteImage;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Application.CarModels.UpdateImage;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.CarModels;

[Route("api/carModels")]
[ApiController]
public class CarModelController : ControllerBase
{
    private readonly ISender _sender;

    public CarModelController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCarModels(CancellationToken cancellationToken, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCarModelsQuery(pageNumber, pageSize);

        Result<Pagination<CarModelResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarModelById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarModelByIdQuery(id);

        Result<CarModelResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCarModel([FromBody] CarModelRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCarModelCommand(request.Brand, request.Model);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCarModelById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCarModel(Guid id, [FromBody] CarModelRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCarModelCommand(id, request.Brand, request.Model);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/image")]
    public async Task<IActionResult> UpdateCarModelImage(Guid id, Uri url, CancellationToken cancellationToken)
    {
        var command = new UpdateCarModelImageCommand(id, url);

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

    [HttpDelete("{id}/image")]
    public async Task<IActionResult> DeleteCarModelImage(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCarModelImageCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

