using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Categories.GetCategories;
using EuroMotors.Application.Categories.GetHierarchicalCategories;
using EuroMotors.Application.Categories.UpdateCategory;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Categories;

[Route("api/categories")]
[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> handler, CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();

        Result<List<CategoryResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("hierarchical")]
    public async Task<IActionResult> GetHierarchicalCategories(IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>> handler, CancellationToken cancellationToken, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetHierarchicalCategoriesQuery(pageNumber, pageSize);

        Result<Pagination<HierarchicalCategoryResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(IQueryHandler<GetCategoryByIdQuery, CategoryResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);

        Result<CategoryResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryRequest request, ICommandHandler<CreateCategoryCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.ParentCategoryId, request.SubcategoryNames, request.Image);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoryById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] UpdateCategoryRequest request, ICommandHandler<UpdateCategoryCommand> handler, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.ParentCategoryId, request.Image);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteCategory(Guid id, ICommandHandler<DeleteCategoryCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
