using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Application.Products.MarkAsNotAvailable;
using EuroMotors.Application.Products.SearchProducts;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Application.Products.UpdateProduct.UpdateProductStock;
using EuroMotors.Domain.Abstractions;
using MediatR;
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
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery();

        Result<IReadOnlyCollection<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<ProductResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? categoryName,
        [FromQuery] string? carModelBrand,
        [FromQuery] string? carModelModel,
        [FromQuery] string? sortOrder,
        [FromQuery] string? searchTerm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new SearchProductsQuery(categoryName, carModelBrand, carModelModel, sortOrder, searchTerm, pageNumber, pageSize);

        Result<Pagination<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Description, request.VendorCode, request.CategoryId, request.CarModelId, request.Price, request.Discount, request.Stock, request.IsAvailable);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description, request.VendorCode, request.CategoryId, request.CarModelId, request.Price, request.Discount, request.Stock, request.IsAvailable);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] int stock, CancellationToken cancellationToken)
    {
        var command = new UpdateProductStockCommand(id, stock);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MarkAsNotAvailable(Guid id, CancellationToken cancellationToken)
    {
        var command = new MarkAsNotAvailableCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}

