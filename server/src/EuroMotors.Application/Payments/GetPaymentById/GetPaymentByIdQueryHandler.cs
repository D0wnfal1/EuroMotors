using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(PaymentResponse.Id)},
                 order_id AS {nameof(PaymentResponse.OrderId)},
                 transaction_id AS {nameof(PaymentResponse.TransactionId)},
                 status AS {nameof(PaymentResponse.Status)},
                 amount AS {nameof(PaymentResponse.Amount)},
                 created_at_utc AS {nameof(PaymentResponse.CreatedAtUtc)}
             FROM payments 
             WHERE id = @PaymentId
             """;

        PaymentResponse? paymentResponse = await connection.QuerySingleOrDefaultAsync<PaymentResponse>(sql, new { request.PaymentId });

        return paymentResponse ?? Result.Failure<PaymentResponse>(PaymentErrors.NotFound(request.PaymentId));
    }
}
