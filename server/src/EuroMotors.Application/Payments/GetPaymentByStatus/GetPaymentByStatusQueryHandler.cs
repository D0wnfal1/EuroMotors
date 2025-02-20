using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.GetPaymentByStatus;

public class GetPaymentByStatusQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetPaymentByStatusQuery, PaymentResponse>
{
	public async Task<Result<PaymentResponse>> Handle(GetPaymentByStatusQuery request, CancellationToken cancellationToken)
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
                 WHERE status = @Status
                 """;

		PaymentResponse? paymentResponse = await connection.QuerySingleOrDefaultAsync<PaymentResponse>(sql, new { request.Status });

		return paymentResponse is null ? Result.Failure<PaymentResponse>(PaymentErrors.NotFoundPaymentStatus(request.Status)) : Result.Success(paymentResponse);
    }
}
