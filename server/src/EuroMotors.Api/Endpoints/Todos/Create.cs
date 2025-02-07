using EuroMotors.Api.Extensions;
using EuroMotors.Api.Infrastructure;
using EuroMotors.Application.Todos.Create;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Todos;
using MediatR;

namespace EuroMotors.Api.Endpoints.Todos;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = [];
        public int Priority { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateTodoCommand
            {
                UserId = request.UserId,
                Description = request.Description,
                DueDate = request.DueDate,
                Labels = request.Labels,
                Priority = (Priority)request.Priority
            };

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok<Guid>, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
