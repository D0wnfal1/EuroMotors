using EuroMotors.Application.Todos.Complete;
using EuroMotors.Application.Todos.Create;
using EuroMotors.Application.Todos.Delete;
using EuroMotors.Application.Todos.Get;
using EuroMotors.Application.Todos.GetById;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Todos;

[Route("api/todos")]
[ApiController]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ISender _sender;

    public TodoController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetTodosQuery(userId);

        Result<List<GetTodoResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTodoByIdQuery(id);

        Result<GetTodoByIdResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTodoCommand()
        {
            UserId = request.UserId,
            Description = request.Description,
            DueDate = request.DueDate,
            Labels = request.Labels,
            Priority = request.Priority
        };
        Result<Guid> result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> CompleteTodo(Guid id, CancellationToken cancellationToken)
    {
        var command = new CompleteTodoCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTodoCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

