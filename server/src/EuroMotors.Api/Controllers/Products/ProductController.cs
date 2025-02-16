using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Application.Products.MarkAsNotAvailable;
using EuroMotors.Application.Products.SearchProductsByCarModelId;
using EuroMotors.Application.Products.SearchProductsByCategoryId;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Application.Products.UpdateProduct.UpdateProductDiscount;
using EuroMotors.Application.Products.UpdateProduct.UpdateProductPrice;
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

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<ProductResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/car-model")]
    public async Task<IActionResult> SearchProductsByCarModelId(Guid id, CancellationToken cancellationToken)
    {
        var query = new SearchProductsByCarModelIdQuery(id);

        Result<IReadOnlyCollection<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/category")]
    public async Task<IActionResult> SearchProductsByCategoryId(Guid id, CancellationToken cancellationToken)
    {
        var query = new SearchProductsByCategoryIdQuery(id);

        Result<IReadOnlyCollection<ProductResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ProductId)
        {
            return BadRequest("ID mismatch");
        }

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/discount")]
    public async Task<IActionResult> UpdateProductDiscount(Guid id, [FromBody] UpdateProductDiscountCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ProductId)
        {
            return BadRequest("ID mismatch");
        }

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdateProductPrice(Guid id, [FromBody] UpdateProductPriceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ProductId)
        {
            return BadRequest("ID mismatch");
        }

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] UpdateProductStockCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ProductId)
        {
            return BadRequest("ID mismatch");
        }

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MarkAsNotAvailable(Guid id, [FromBody] MarkAsNotAvailableCommand command, CancellationToken cancellationToken)
    {
        if (id != command.ProductId)
        {
            return BadRequest("ID mismatch");
        }

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

