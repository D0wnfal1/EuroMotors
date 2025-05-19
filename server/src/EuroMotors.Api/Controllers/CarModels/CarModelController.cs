using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetAllCarModelBrands;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.CarModels.GetCarModelSelection;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.CarModels;

[Route("api/carModels")]
[ApiController]
public sealed class CarModelController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCarModels(IQueryHandler<GetCarModelsQuery, Pagination<CarModelResponse>> handler, CancellationToken cancellationToken, [FromQuery] Guid? brandId = null,
        [FromQuery] string? searchTerm = null, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCarModelsQuery(brandId, searchTerm, pageNumber, pageSize);

        Result<Pagination<CarModelResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarModelById(IQueryHandler<GetCarModelByIdQuery, CarModelResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarModelByIdQuery(id);

        Result<CarModelResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetAllBrands(IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>> handler, CancellationToken cancellationToken)
    {
        var query = new GetAllCarModelBrandsQuery();
        Result<List<CarBrandResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("selection")]
    public async Task<IActionResult> GetCarSelection([FromQuery] SelectCarModelRequest request, IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse> handler, CancellationToken cancellationToken)
    {
        var query = new GetCarModelSelectionQuery(
            request.BrandId,
            request.Brand,
            request.Model,
            request.StartYear,
            request.BodyType);

        Result<CarModelSelectionResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateCarModel([FromForm] CreateCarModelRequest request, ICommandHandler<CreateCarModelCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var engineSpec = new EngineSpec(request.VolumeLiters, request.FuelType);

        var command = new CreateCarModelCommand(
            request.CarBrandId,
            request.ModelName,
            request.StartYear,
            request.BodyType,
            engineSpec
        );

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCarModelById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateCarModel(
        Guid id,
        [FromForm] UpdateCarModelRequest request,
        ICommandHandler<UpdateCarModelCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCarModelCommand(
            id,
            request.Model,
            request.StartYear,
            request.BodyType,
            request.VolumeLiters,
            request.FuelType);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteCarModel(Guid id, ICommandHandler<DeleteCarModelCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteCarModelCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

