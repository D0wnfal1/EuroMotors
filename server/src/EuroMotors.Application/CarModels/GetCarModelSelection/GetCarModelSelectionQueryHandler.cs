﻿using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetCarModelSelection;

internal sealed class GetCarModelSelectionQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService) : IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<Result<CarModelSelectionResponse>> Handle(GetCarModelSelectionQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CreateCacheKey(request);

        CarModelSelectionResponse? cachedResult = await cacheService.GetAsync<CarModelSelectionResponse>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result.Success(cachedResult);
        }

        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        Guid? brandIdFromName = null;
        if (!string.IsNullOrEmpty(request.Brand))
        {
            string brandIdSql = "SELECT id FROM car_brands WHERE name = @Brand";
            brandIdFromName = await connection.QueryFirstOrDefaultAsync<Guid?>(brandIdSql, new { request.Brand });
        }

        Guid? effectiveBrandId = request.BrandId ?? brandIdFromName;

        string sql = @"
            SELECT DISTINCT cm.id 
            FROM car_models cm
            JOIN car_brands cb ON cm.car_brand_id = cb.id
            WHERE (@BrandId IS NULL OR cm.car_brand_id = @BrandId)
                AND (@ModelName IS NULL OR cm.model_name = @ModelName)
                AND (@StartYear IS NULL OR cm.start_year = @StartYear)
                AND (@BodyType IS NULL OR cm.body_type = @BodyType);

            SELECT DISTINCT cb.id, cb.name 
            FROM car_brands cb
            JOIN car_models cm ON cb.id = cm.car_brand_id
            WHERE (@BrandId IS NULL OR cb.id = @BrandId)
            ORDER BY cb.name;
    
            SELECT DISTINCT cm.model_name 
            FROM car_models cm
            WHERE (@ModelName IS NULL OR cm.model_name = @ModelName) 
                AND (@BrandId IS NULL OR cm.car_brand_id = @BrandId)
            ORDER BY cm.model_name;
    
            SELECT DISTINCT cm.start_year 
            FROM car_models cm
            WHERE (@StartYear IS NULL OR cm.start_year = @StartYear) 
                AND (@BrandId IS NULL OR cm.car_brand_id = @BrandId)
                AND (@ModelName IS NULL OR cm.model_name = @ModelName)
            ORDER BY cm.start_year;
    
            SELECT DISTINCT cm.body_type 
            FROM car_models cm
            WHERE (@BodyType IS NULL OR cm.body_type = @BodyType)
                AND (@BrandId IS NULL OR cm.car_brand_id = @BrandId)
                AND (@ModelName IS NULL OR cm.model_name = @ModelName)
            ORDER BY cm.body_type;

            SELECT DISTINCT 
                CONCAT(cm.engine_spec_volume_liters, 'L ', cm.engine_spec_fuel_type) AS EngineSpec
            FROM car_models cm
            WHERE (@BrandId IS NULL OR cm.car_brand_id = @BrandId)
                AND (@ModelName IS NULL OR cm.model_name = @ModelName)
                AND (@StartYear IS NULL OR cm.start_year = @StartYear)
                AND (@BodyType IS NULL OR cm.body_type = @BodyType)
            ORDER BY EngineSpec;
            ";

        await using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, new
        {
            BrandId = effectiveBrandId,
            request.ModelName,
            request.StartYear,
            request.BodyType,
        });

        IEnumerable<Guid> ids = await multi.ReadAsync<Guid>();
        IEnumerable<BrandInfo> brands = await multi.ReadAsync<BrandInfo>();
        IEnumerable<string> models = await multi.ReadAsync<string>();
        IEnumerable<int> years = await multi.ReadAsync<int>();
        IEnumerable<string> bodyTypes = await multi.ReadAsync<string>();
        IEnumerable<string> engineSpecs = await multi.ReadAsync<string>();

        var result = new CarModelSelectionResponse()
        {
            Ids = ids.ToList(),
            Brands = brands.ToList(),
            Models = models.ToList(),
            Years = years.ToList(),
            BodyTypes = bodyTypes.ToList(),
            EngineSpecs = engineSpecs.ToList(),
        };

        await cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return Result.Success(result);
    }

    private static string CreateCacheKey(GetCarModelSelectionQuery request)
    {
        string baseKey = CacheKeys.CarModels.GetSelection();

        string brandId = request.BrandId?.ToString() ?? "none";
        string brand = !string.IsNullOrEmpty(request.Brand) ? request.Brand : "none";
        string modelName = !string.IsNullOrEmpty(request.ModelName) ? request.ModelName : "none";
#pragma warning disable CA1305
        string startYear = request.StartYear?.ToString() ?? "none";
#pragma warning restore CA1305
        string bodyType = !string.IsNullOrEmpty(request.BodyType) ? request.BodyType : "none";

        return $"{baseKey}:{brandId}:{brand}:{modelName}:{startYear}:{bodyType}";
    }
}
