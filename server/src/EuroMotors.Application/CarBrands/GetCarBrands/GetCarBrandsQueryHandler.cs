using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarBrands.GetCarBrands;

internal sealed class GetCarBrandsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>>
{
    public async Task<Result<Pagination<CarBrandResponse>>> Handle(GetCarBrandsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine(
          $"""
            SELECT
                id AS {nameof(CarBrandResponse.Id)},
                name AS {nameof(CarBrandResponse.Name)},
                slug AS {nameof(CarBrandResponse.Slug)},
                logo_path AS {nameof(CarBrandResponse.LogoPath)}
            FROM car_brands
            ORDER BY name
            """);

        var parameters = new Dictionary<string, object>();

        if (request.PageSize > 0)
        {
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            sql.AppendLine("LIMIT @PageSize OFFSET @Offset");
        }

        List<CarBrandResponse> carBrands = (await connection.QueryAsync<CarBrandResponse>(sql.ToString(), parameters)).AsList();

        string countSql = "SELECT COUNT(*) FROM car_brands";

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql);

        if (request.PageSize > 0)
        {
            var paginatedResult = new Pagination<CarBrandResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = carBrands
            };

            return Result.Success(paginatedResult);
        }

        var result = new Pagination<CarBrandResponse>
        {
            PageIndex = 1,
            PageSize = totalCount,
            Count = totalCount,
            Data = carBrands
        };

        return Result.Success(result);
    }
}
