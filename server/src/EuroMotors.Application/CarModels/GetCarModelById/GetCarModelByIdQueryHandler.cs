using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.GetCarModelById;

internal sealed class GetCarModelByIdQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCarModelByIdQuery, CarModelResponse>
{
    public async Task<Result<CarModelResponse>> Handle(GetCarModelByIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
                SELECT
                    id AS {nameof(CarModelResponse.Id)},
                    brand AS {nameof(CarModelResponse.Brand)},
                    model AS {nameof(CarModelResponse.Model)}
                FROM car_models
                WHERE id = @CarModelId
                """;

        CarModelResponse? carModel = await connection.QuerySingleOrDefaultAsync<CarModelResponse>(sql, new { request.CarModelId });

        return carModel ?? Result.Failure<CarModelResponse>(CarModelErrors.NotFound(request.CarModelId));
    }
}
