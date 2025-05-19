using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarBrands.DeleteCarBrand;
using EuroMotors.Application.CarBrands.GetCarBrandById;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.CarBrands.UpdateCarBrand;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.CarBrands;

[Route("api/carBrands")]
[ApiController]
public sealed class CarBrandController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCarBrands(IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>> handler, CancellationToken cancellationToken, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCarBrandsQuery(pageNumber, pageSize);

        Result<Pagination<CarBrandResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCarBrandById(IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCarBrandByIdQuery(id);

        Result<CarBrandResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateCarBrand([FromForm] CreateCarBrandRequest request, ICommandHandler<CreateCarBrandCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new CreateCarBrandCommand(
            request.Name,
            request.Logo);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCarBrandById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateCarBrand(
        Guid id,
        [FromForm] UpdateCarBrandRequest request,
        ICommandHandler<UpdateCarBrandCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCarBrandCommand(
            id,
            request.Name,
            request.Logo);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteCarBrand(Guid id, ICommandHandler<DeleteCarBrandCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteCarBrandCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
