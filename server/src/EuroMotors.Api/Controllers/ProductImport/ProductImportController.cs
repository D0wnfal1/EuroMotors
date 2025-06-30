using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImport.ImportExcelProducts;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.ProductImport;

[ApiController]
[Route("api/product-import")]
public class ProductImportController : ControllerBase
{
    [HttpPost("excel")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ImportExcelProducts(
        IFormFile priceFile,
        IFormFile? mappingFile,
        ICommandHandler<ImportExcelProductsCommand, ImportExcelProductsResult> handler,
        CancellationToken cancellationToken)
    {
        if (priceFile == null || priceFile.Length == 0)
        {
            return BadRequest("Price file is required");
        }

#pragma warning disable S1125
        if (priceFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) == false)
#pragma warning restore S1125
        {
            return BadRequest("Price file must be an Excel file (.xlsx)");
        }

        if (mappingFile != null && mappingFile.Length > 0 && 
#pragma warning disable S1125
            mappingFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) == false)
#pragma warning restore S1125
        {
            return BadRequest("Mapping file must be an Excel file (.xlsx)");
        }

        using Stream priceStream = priceFile.OpenReadStream();
        using Stream? mappingStream = mappingFile?.OpenReadStream();

        var command = new ImportExcelProductsCommand(priceStream, mappingStream);
        Result<ImportExcelProductsResult> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
} 
