using EuroMotors.Application.Callback.RequestCallback;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Callback;
[Route("api/callback")]
[ApiController]
public class CallbackController : ControllerBase
{
    private readonly ISender _sender;

    public CallbackController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Route("request")]
    public async Task<IActionResult> RequestCallback([FromBody] CallbackRequest request)
    {
        var command = new RequestCallbackCommand(request.Name, request.Phone);
        Result result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound();
    }
}
