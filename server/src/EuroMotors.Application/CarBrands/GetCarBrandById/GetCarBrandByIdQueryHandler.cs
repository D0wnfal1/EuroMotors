using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarBrands.GetCarBrandById;

internal sealed class GetCarBrandByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>
{
    public async Task<Result<CarBrandResponse>> Handle(GetCarBrandByIdQuery request, CancellationToken cancellationToken)
    {
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

        return carBrand ?? Result.Failure<CarBrandResponse>(CarModelErrors.BrandNotFound(request.CarBrandId));
    }
}