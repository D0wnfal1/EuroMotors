using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetAllCarModelBrands;

internal sealed class GetAllCarModelBrandsQueryHandler(
    IDbConnectionFactory dbConnectionFactory
) : IQueryHandler<GetAllCarModelBrandsQuery, List<string>>
{
    public async Task<Result<List<string>>> Handle(GetAllCarModelBrandsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql = "SELECT DISTINCT brand FROM car_models ORDER BY brand;";

        IEnumerable<string> brands = await connection.QueryAsync<string>(sql);

        return brands.ToList();
    }
}
