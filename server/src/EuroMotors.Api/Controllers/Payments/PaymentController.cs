using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Application.Payments.CreatePayment;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Application.Payments.GetPaymentByOrderId;
using EuroMotors.Application.Payments.GetPaymentByStatus;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Payments;

[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);

        Result<PaymentResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/order")]
    public async Task<IActionResult> GetPaymentByOrderId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByOrderIdQuery(id);

        Result<PaymentResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetPaymentByStatus(PaymentStatus status, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByStatusQuery(status);

        Result<PaymentResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

