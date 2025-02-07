using EuroMotors.Api.Extensions;
using EuroMotors.Api.Infrastructure;
using EuroMotors.Application.Users.Login;
using EuroMotors.Domain.Abstractions;
using MediatR;

namespace EuroMotors.Api.Endpoints.Users;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);

            Result<string> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok<string>, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
