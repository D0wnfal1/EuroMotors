using EuroMotors.Api.Extensions;
using EuroMotors.Api.Infrastructure;
using EuroMotors.Application.Todos.Get;
using EuroMotors.Domain.Abstractions;
using MediatR;

namespace EuroMotors.Api.Endpoints.Todos;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos", async (Guid userId, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new GetTodosQuery(userId);

            Result<List<TodoResponse>> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
