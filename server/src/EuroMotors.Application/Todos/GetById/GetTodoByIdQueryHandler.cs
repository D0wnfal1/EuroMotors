using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Todos;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Application.Todos.GetById;

internal sealed class GetTodoByIdQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodoByIdQuery, GetTodoByIdResponse>
{
    public async Task<Result<GetTodoByIdResponse>> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        GetTodoByIdResponse? todo = await context.TodoItems
            .Where(todoItem => todoItem.Id == query.TodoItemId && todoItem.UserId == userContext.UserId)
            .Select(todoItem => new GetTodoByIdResponse
            {
                Id = todoItem.Id,
                UserId = todoItem.UserId,
                Description = todoItem.Description,
                DueDate = todoItem.DueDate,
                Labels = todoItem.Labels,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                CompletedAt = todoItem.CompletedAt
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (todo is null)
        {
            return Result.Failure<GetTodoByIdResponse>(TodoItemErrors.NotFound(query.TodoItemId));
        }

        return todo;
    }
}
