using EuroMotors.Api.Extensions;
using EuroMotors.Api.Infrastructure;
using EuroMotors.Application.Todos.GetById;
using EuroMotors.Domain.Abstractions;
using MediatR;

namespace EuroMotors.Api.Endpoints.Todos;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new GetTodoByIdQuery(id);

            Result<TodoResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
