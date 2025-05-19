using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImages.DeleteProductImage;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.ProductImages;

[Route("api/productImages")]
[ApiController]
public class ProductImageController : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UploadProductImage([FromForm] UploadProductImageRequest request, ICommandHandler<UploadProductImageCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new UploadProductImageCommand(request.File, request.ProductId);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(UploadProductImage), new { id = result.Value }, new { id = result.Value })
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateProductImage(Guid id, [FromBody] UploadProductImageRequest request, ICommandHandler<UpdateProductImageCommand> handler, CancellationToken cancellationToken)
    {
        var command = new UpdateProductImageCommand(id, request.File, request.ProductId);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteProductImage(Guid id, ICommandHandler<DeleteProductImageCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
