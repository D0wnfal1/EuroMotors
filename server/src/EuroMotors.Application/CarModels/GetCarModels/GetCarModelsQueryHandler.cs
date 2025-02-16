using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetCarModels;

public class GetCarModelsQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCarModelsQuery, IReadOnlyCollection<CarModelResponse>>
{
    public async Task<Result<IReadOnlyCollection<CarModelResponse>>> Handle(GetCarModelsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql = 
           $"""
            SELECT
                id AS {nameof(CarModelResponse.Id)},
                brand AS {nameof(CarModelResponse.Brand)},
                model AS {nameof(CarModelResponse.Model)},
            FROM car_models
            """;

        List<CarModelResponse> carModels = (await connection.QueryAsync<CarModelResponse>(sql, request)).AsList();

        return carModels;
    }
}
