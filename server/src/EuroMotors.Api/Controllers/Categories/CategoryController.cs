using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.ArchiveCategory;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Categories.GetCategories;
using EuroMotors.Application.Categories.UpdateCategory;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Categories;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCategoriesQuery(pageNumber, pageSize);

        Result<Pagination<CategoryResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);

        Result<CategoryResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.ParentCategoryId, request.SubcategoryNames, request.Image);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoryById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.Image);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{id}/archive")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ArchiveCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new ArchiveCategoryCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    //[Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
