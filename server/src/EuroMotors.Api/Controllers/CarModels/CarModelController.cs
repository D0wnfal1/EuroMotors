using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.CarModels.GetCarModelSelection;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetCarModelById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarModelByIdQuery(id);

        Result<CarModelResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("selection")]
    public async Task<IActionResult> GetCarSelection([FromQuery] SelectCarModelRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCarModelSelectionQuery(request.Brand, request.Model, request.StartYear, request.BodyType);

        Result<CarModelSelectionResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateCarModel([FromForm] CreateCarModelRequest request, CancellationToken cancellationToken)
    {
        var engineSpec = new EngineSpec(request.VolumeLiters, request.FuelType);

        var command = new CreateCarModelCommand(
            request.Brand,
            request.Model,
            request.StartYear,
            request.BodyType,
            engineSpec,
            request.ImagePath
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCarModelById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateCarModel(
        Guid id,
        [FromForm] UpdateCarModelRequest request,
        CancellationToken cancellationToken)
    {
            var command = new UpdateCarModelCommand(id,
            request.Brand,
            request.Model,
            request.StartYear,
            request.BodyType,
            request.VolumeLiters,
            request.FuelType,
            request.ImagePath);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteCarModel(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCarModelCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

