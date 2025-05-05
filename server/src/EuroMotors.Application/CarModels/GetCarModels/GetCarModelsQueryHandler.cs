using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetCarModels;

internal sealed class GetCarModelsQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService) : IQueryHandler<GetCarModelsQuery, Pagination<CarModelResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<Result<Pagination<CarModelResponse>>> Handle(GetCarModelsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CreateCacheKey(request);

        Pagination<CarModelResponse>? cachedResult = await cacheService.GetAsync<Pagination<CarModelResponse>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result.Success(cachedResult);
        }

        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine(
          $"""
            SELECT
                cm.id AS {nameof(CarModelResponse.Id)},
                cm.car_brand_id AS {nameof(CarModelResponse.CarBrandId)},
                cb.name AS {nameof(CarModelResponse.BrandName)},
                cm.model_name AS {nameof(CarModelResponse.ModelName)},
                cm.start_year AS {nameof(CarModelResponse.StartYear)},
                cm.body_type AS {nameof(CarModelResponse.BodyType)},
                cm.engine_spec_volume_liters AS {nameof(CarModelResponse.VolumeLiters)},
                cm.engine_spec_fuel_type AS {nameof(CarModelResponse.FuelType)},
                cm.slug AS {nameof(CarModelResponse.Slug)}
            FROM car_models cm
            JOIN car_brands cb ON cm.car_brand_id = cb.id
            """);

        var whereClause = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (request.BrandId.HasValue)
        {
            whereClause.Add("cm.car_brand_id = @BrandId");
            parameters.Add("BrandId", request.BrandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            whereClause.Add("(cb.name ILIKE @SearchTerm OR cm.model_name ILIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
        }

        if (whereClause.Count > 0)
        {
            sql.AppendLine(string.Format(CultureInfo.InvariantCulture, "WHERE {0}", string.Join(" AND ", whereClause)));
        }

        sql.AppendLine("ORDER BY cb.name, cm.model_name");

        if (request.PageSize > 0)
        {
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            sql.AppendLine("LIMIT @PageSize OFFSET @Offset");
        }

        List<CarModelResponse> carModels = (await connection.QueryAsync<CarModelResponse>(sql.ToString(), parameters)).AsList();

        var countSql = new StringBuilder();
        countSql.Append("SELECT COUNT(*) FROM car_models cm ");

        if (request.BrandId.HasValue)
        {
            countSql.Append("WHERE cm.car_brand_id = @BrandId");
        }

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters);

        Pagination<CarModelResponse> result;

        result = request.PageSize > 0
            ? new Pagination<CarModelResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = carModels
            }
            : new Pagination<CarModelResponse>
            {
                PageIndex = 1,
                PageSize = totalCount,
                Count = totalCount,
                Data = carModels
            };

        await cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return Result.Success(result);
    }

    private static string CreateCacheKey(GetCarModelsQuery request)
    {
        string baseKey = CacheKeys.CarModels.GetList();

        string brandId = request.BrandId?.ToString() ?? "all";
        string searchTerm = !string.IsNullOrEmpty(request.SearchTerm) ? request.SearchTerm : "none";
        string pagination = $"{request.PageNumber}_{request.PageSize}";

        return $"{baseKey}:{brandId}:{searchTerm}:{pagination}";
    }
}
