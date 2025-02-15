using EuroMotors.Application.ProductImages.GetProductImageById;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.ProductImages;

[Route("api/product_images")]
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

        return result.IsSuccess ? Ok(result) : NotFound();
    }
}
