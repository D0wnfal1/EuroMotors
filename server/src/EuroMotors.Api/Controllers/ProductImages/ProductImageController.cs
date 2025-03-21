using EuroMotors.Application.ProductImages.CreateProductImage;
using EuroMotors.Application.ProductImages.DeleteProductImage;
using EuroMotors.Application.ProductImages.DeleteProductImagesByProductId;
using EuroMotors.Application.ProductImages.GetProductImageById;
using EuroMotors.Application.ProductImages.GetProductImagesByProductId;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.ProductImages;

[Route("api/productImages")]
[ApiController]
public class ProductImageController : ControllerBase
{
    private readonly ISender _sender;

    public ProductImageController(ISender sender)
    {
        _sender = sender;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductImageById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductImageByIdQuery(id);

        Result<ProductImageResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("{id}/product")]
    public async Task<IActionResult> GetProductImageByProductId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductImagesByProductIdQuery(id);

        Result<IReadOnlyCollection<ProductImageResponse>> result = await _sender.Send(query, cancellationToken);

        return Ok(result.IsSuccess ? result.Value : new List<ProductImageResponse>());
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProductImage([FromForm] UploadProductImageRequest request, CancellationToken cancellationToken)
    {
        var command = new UploadProductImageCommand(request.File, request.ProductId);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductImageById), new { id = result.Value }, new { id = result.Value })
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductImage(Guid id, [FromBody] UploadProductImageRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductImageCommand(id, request.File, request.ProductId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductImage(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpDelete("{id}/product")]
    public async Task<IActionResult> DeleteProductImageByProductId(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageByProductIdCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
