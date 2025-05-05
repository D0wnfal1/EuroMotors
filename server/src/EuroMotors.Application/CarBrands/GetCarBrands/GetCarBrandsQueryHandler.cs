using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarBrands.GetCarBrands;

internal sealed class GetCarBrandsQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<Result<Pagination<CarBrandResponse>>> Handle(GetCarBrandsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CreateCacheKey(request);

        Pagination<CarBrandResponse>? cachedResult = await cacheService.GetAsync<Pagination<CarBrandResponse>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result.Success(cachedResult);
        }

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

        Pagination<CarBrandResponse> result;

        result = request.PageSize > 0
            ? new Pagination<CarBrandResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = carBrands
            }
            : new Pagination<CarBrandResponse>
            {
                PageIndex = 1,
                PageSize = totalCount,
                Count = totalCount,
                Data = carBrands
            };

        await cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return Result.Success(result);
    }

    private static string CreateCacheKey(GetCarBrandsQuery request)
    {
        string baseKey = CacheKeys.CarBrands.GetList();

        string pagination = $"{request.PageNumber}_{request.PageSize}";

        return $"{baseKey}:{pagination}";
    }
}
