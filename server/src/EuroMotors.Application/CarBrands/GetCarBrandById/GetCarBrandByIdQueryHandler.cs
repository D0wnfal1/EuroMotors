using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarBrands.GetCarBrandById;

internal sealed class GetCarBrandByIdQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);
    
    public async Task<Result<CarBrandResponse>> Handle(GetCarBrandByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.CarBrands.GetById(request.CarBrandId);
        
        CarBrandResponse? cachedCarBrand = await cacheService.GetAsync<CarBrandResponse>(cacheKey, cancellationToken);
        if (cachedCarBrand != null)
        {
            return Result.Success(cachedCarBrand);
        }
        
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
                SELECT
                    id AS {nameof(CarBrandResponse.Id)},
                    name AS {nameof(CarBrandResponse.Name)},
                    slug AS {nameof(CarBrandResponse.Slug)},
                    logo_path AS {nameof(CarBrandResponse.LogoPath)}
                FROM car_brands
                WHERE id = @CarBrandId
                """;

        CarBrandResponse? carBrand = await connection.QuerySingleOrDefaultAsync<CarBrandResponse>(
            sql, new { request.CarBrandId });

        if (carBrand == null)
        {
            return Result.Failure<CarBrandResponse>(CarModelErrors.BrandNotFound(request.CarBrandId));
        }
        
        await cacheService.SetAsync(cacheKey, carBrand, CacheExpiration, cancellationToken);

        return Result.Success(carBrand);
    }
}
