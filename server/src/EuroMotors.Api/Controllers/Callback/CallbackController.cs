using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Callback.RequestCallback;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Callback;
[Route("api/callback")]
[ApiController]
public class CallbackController : ControllerBase
{
    [HttpPost]
    [Route("request")]
    public async Task<IActionResult> RequestCallback([FromBody] CallbackRequest request, ICommandHandler<RequestCallbackCommand> handler, CancellationToken cancellationToken)
    {
        var command = new RequestCallbackCommand(request.Name, request.Phone);
        Result result = await handler.Handle(command, cancellationToken);
        return result.IsSuccess ? Ok(result) : NotFound();
    }
}
