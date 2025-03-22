using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetCarModels;

internal sealed class GetCarModelsQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCarModelsQuery, Pagination<CarModelResponse>>
{
    public async Task<Result<Pagination<CarModelResponse>>> Handle(GetCarModelsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine(
          $"""
            SELECT
                id AS {nameof(CarModelResponse.Id)},
                brand AS {nameof(CarModelResponse.Brand)},
                model AS {nameof(CarModelResponse.Model)},
                image_url AS {nameof(CarModelResponse.ImageUrl)}
            FROM car_models
            """);

        var parameters = new Dictionary<string, object>();

        if (request.PageSize > 0)
        {
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            sql.AppendLine("LIMIT @PageSize OFFSET @Offset");
        }

        List<CarModelResponse> carModels = (await connection.QueryAsync<CarModelResponse>(sql.ToString(), parameters)).AsList();

        var countSql = new StringBuilder();
        countSql.Append("SELECT COUNT(*) FROM car_models ");

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString());

        if (request.PageSize > 0)
        {
            var paginatedResult = new Pagination<CarModelResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = carModels
            };

            return Result.Success(paginatedResult);
        }

        var result = new Pagination<CarModelResponse>
        {
            PageIndex = 1,
            PageSize = totalCount,
            Count = totalCount,
            Data = carModels
        };

        return Result.Success(result);
    }
}
