using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.CopyProduct;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Application.Products.GetProductsByBrandName;
using EuroMotors.Application.Products.GetProductsByCategoryWithChildren;
using EuroMotors.Application.Products.ImportProducts;
using EuroMotors.Application.Products.SetProductAvailability;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Application.Products.UpdateProductCarModels;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Products;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] List<Guid>? categoryIds,
        [FromQuery] List<Guid>? carModelIds,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isDiscounted,
        [FromQuery] bool? isNew,
        [FromQuery] bool? isPopular,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery(
            categoryIds,
            carModelIds,
            sortOrder,
            searchTerm,
            isDiscounted,
            isNew,
            isPopular,
            pageNumber,
            pageSize);

        Result<Pagination<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<ProductResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Specifications.Select(s => new Specification(s.SpecificationName, s.SpecificationValue)).ToList(),
            request.VendorCode,
            request.CategoryId,
            request.CarModelIds,
            request.Price,
            request.Discount,
            request.Stock
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{id}/copy")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CopyProduct(Guid id, CancellationToken cancellationToken)
    {
        var command = new CopyProductCommand(id);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Specifications.Select(s => new Specification(s.SpecificationName, s.SpecificationValue)).ToList(),
            request.VendorCode,
            request.CategoryId,
            request.Price,
            request.Discount,
            request.Stock
        );

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetProductAvailability(Guid id, [FromBody] SetProductAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var command = new SetProductAvailabilityCommand(id, request.IsAvailable);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpPut("{productId}/car-models")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProductCarModels(Guid productId, [FromBody] List<Guid> carModelIds, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCarModelsCommand(productId, carModelIds);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("by-brand-name")]
    public async Task<IActionResult> GetProductsByBrandName(
        [FromQuery] string brandName,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsByBrandNameQuery(brandName, sortOrder, searchTerm, pageNumber, pageSize);

        Result<Pagination<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategoryWithChildren(
        Guid categoryId,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsByCategoryWithChildrenQuery(categoryId, sortOrder, searchTerm, pageNumber, pageSize);

        Result<Pagination<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("import")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ImportProducts(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are allowed.");
        }

        using var stream = file.OpenReadStream();
        var command = new ImportProductsCommand(stream, file.FileName);

        Result<ImportProductsResult> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

