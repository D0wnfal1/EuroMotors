using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Payments.CreatePayment;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Application.Payments.GetPaymentByOrderId;
using EuroMotors.Application.Payments.GetPaymentByStatus;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Payments;

[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetPaymentById(IQueryHandler<GetPaymentByIdQuery, PaymentResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);

        Result<PaymentResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}/order")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetPaymentByOrderId(IQueryHandler<GetPaymentByOrderIdQuery, PaymentResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByOrderIdQuery(id);

        Result<PaymentResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetPaymentByStatus(IQueryHandler<GetPaymentByStatusQuery, PaymentResponse> handler, PaymentStatus status, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByStatusQuery(status);

        Result<PaymentResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromQuery] Guid orderId, ICommandHandler<CreatePaymentCommand, Dictionary<string, string>> handler, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(orderId);

        Result<Dictionary<string, string>> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

