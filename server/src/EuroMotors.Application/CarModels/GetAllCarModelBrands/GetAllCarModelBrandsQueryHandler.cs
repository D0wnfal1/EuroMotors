using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetAllCarModelBrands;

internal sealed class GetAllCarModelBrandsQueryHandler(
    IDbConnectionFactory dbConnectionFactory
) : IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>>
{
    public async Task<Result<List<CarBrandResponse>>> Handle(GetAllCarModelBrandsQuery request, CancellationToken cancellationToken)
    {
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

        return brands.ToList();
    }
}
