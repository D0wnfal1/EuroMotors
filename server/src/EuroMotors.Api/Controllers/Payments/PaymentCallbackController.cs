using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Payments;

[Route("api/payments/callback")]
[ApiController]
public class PaymentCallbackController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PaymentCallback([FromForm] string data, [FromForm] string signature)
    {
        Result result = await paymentService.ProcessPaymentCallbackAsync(data, signature);

        return result.IsFailure ? BadRequest(result.Error) : Ok(result);
    }
}
