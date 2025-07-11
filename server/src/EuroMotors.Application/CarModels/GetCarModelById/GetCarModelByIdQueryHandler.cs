﻿using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.GetCarModelById;

internal sealed class GetCarModelByIdQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService) : IQueryHandler<GetCarModelByIdQuery, CarModelResponse>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<Result<CarModelResponse>> Handle(GetCarModelByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.CarModels.GetById(request.CarModelId);

        CarModelResponse? cachedCarModel = await cacheService.GetAsync<CarModelResponse>(cacheKey, cancellationToken);
        if (cachedCarModel != null)
        {
            return Result.Success(cachedCarModel);
        }

        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
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
                WHERE cm.id = @CarModelId
                """;

        CarModelResponse? carModel = await connection.QuerySingleOrDefaultAsync<CarModelResponse>(sql, new { request.CarModelId });

        if (carModel == null)
        {
            return Result.Failure<CarModelResponse>(CarModelErrors.ModelNotFound(request.CarModelId));
        }

        await cacheService.SetAsync(cacheKey, carModel, CacheExpiration, cancellationToken);

        return Result.Success(carModel);
    }
}
