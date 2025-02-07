using EuroMotors.Api.Extensions;
using EuroMotors.Api.Infrastructure;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using MediatR;

namespace EuroMotors.Api.Endpoints.Users;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok<Guid>, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
