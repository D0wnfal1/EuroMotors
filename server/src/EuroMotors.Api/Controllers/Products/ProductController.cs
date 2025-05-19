using EuroMotors.Application.Abstractions.Messaging;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Products;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        IQueryHandler<GetProductsQuery, Pagination<ProductResponse>> handler,
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

        Result<Pagination<ProductResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(IQueryHandler<GetProductByIdQuery, ProductResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<ProductResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, ICommandHandler<CreateProductCommand, Guid> handler, CancellationToken cancellationToken)
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

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("{id}/copy")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CopyProduct(Guid id, ICommandHandler<CopyProductCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new CopyProductCommand(id);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, ICommandHandler<UpdateProductCommand> handler, CancellationToken cancellationToken)
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

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetProductAvailability(Guid id, [FromBody] SetProductAvailabilityRequest request, ICommandHandler<SetProductAvailabilityCommand> handler, CancellationToken cancellationToken)
    {
        var command = new SetProductAvailabilityCommand(id, request.IsAvailable);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteProduct(Guid id, ICommandHandler<DeleteProductCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpPut("{productId}/car-models")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProductCarModels(Guid productId, [FromBody] List<Guid> carModelIds, ICommandHandler<UpdateProductCarModelsCommand> handler, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCarModelsCommand(productId, carModelIds);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("by-brand-name")]
    public async Task<IActionResult> GetProductsByBrandName(
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler,
        [FromQuery] string brandName,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsByBrandNameQuery(brandName, sortOrder, searchTerm, pageNumber, pageSize);

        Result<Pagination<ProductResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategoryWithChildren(
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler,
        Guid categoryId,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsByCategoryWithChildrenQuery(categoryId, sortOrder, searchTerm, pageNumber, pageSize);

        Result<Pagination<ProductResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("import")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ImportProducts(IFormFile file, ICommandHandler<ImportProductsCommand, ImportProductsResult> handler, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are allowed.");
        }

        using Stream stream = file.OpenReadStream();
        var command = new ImportProductsCommand(stream, file.FileName);

        Result<ImportProductsResult> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

