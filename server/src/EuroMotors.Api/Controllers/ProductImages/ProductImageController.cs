using EuroMotors.Application.ProductImages.DeleteProductImage;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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


    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UploadProductImage([FromForm] UploadProductImageRequest request, CancellationToken cancellationToken)
    {
        var command = new UploadProductImageCommand(request.File, request.ProductId);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(UploadProductImage), new { id = result.Value }, new { id = result.Value })  
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProductImage(Guid id, [FromBody] UploadProductImageRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductImageCommand(id, request.File, request.ProductId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteProductImage(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
