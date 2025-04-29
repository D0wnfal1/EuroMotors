using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetAllCarModelBrands;

internal sealed class GetAllCarModelBrandsQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService
) : IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);
    
    public async Task<Result<List<CarBrandResponse>>> Handle(GetAllCarModelBrandsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.CarBrands.GetAllForModels();
        
        List<CarBrandResponse>? cachedBrands = await cacheService.GetAsync<List<CarBrandResponse>>(cacheKey, cancellationToken);
        if (cachedBrands != null)
        {
            return Result.Success(cachedBrands);
        }
        
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql = @"
            SELECT 
                id AS Id, 
                name AS Name,
                slug AS Slug,
                logo_path AS LogoPath
            FROM car_brands 
            ORDER BY name;";

        IEnumerable<CarBrandResponse> brands = await connection.QueryAsync<CarBrandResponse>(sql);
        
        var result = brands.ToList();
        
        await cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return Result.Success(result);
    }
}
