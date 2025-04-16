using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.CarModels.GetCarModelSelection;

internal sealed class GetCarModelSelectionQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse>
{
    public async Task<Result<CarModelSelectionResponse>> Handle(GetCarModelSelectionQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();
        string sql = @"
            SELECT DISTINCT brand 
            FROM car_models 
            WHERE (@Brand IS NULL OR brand = @Brand);
    
            SELECT DISTINCT model 
            FROM car_models 
            WHERE (@Model IS NULL OR model = @Model) 
                AND (@Brand IS NULL OR brand = @Brand);
    
            SELECT DISTINCT start_year 
            FROM car_models 
            WHERE (@StartYear IS NULL OR start_year = @StartYear) 
                AND (@Brand IS NULL OR brand = @Brand)
                AND (@Model IS NULL OR model = @Model);
    
            SELECT DISTINCT body_type 
            FROM car_models 
            WHERE (@BodyType IS NULL OR body_type = @BodyType)
                AND (@Brand IS NULL OR brand = @Brand)
                AND (@Model IS NULL OR model = @Model);

            SELECT DISTINCT 
                CONCAT(engine_spec_volume_liters, ' ', engine_spec_fuel_type) AS EngineSpec
            FROM car_models 
            WHERE (@Brand IS NULL OR brand = @Brand)
                AND (@Model IS NULL OR model = @Model)
                AND (@StartYear IS NULL OR start_year = @StartYear)
                AND (@BodyType IS NULL OR body_type = @BodyType);
            ";

        await using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, new
        {
            request.Brand,
            request.Model,
            request.StartYear,
            request.BodyType,
        });

        IEnumerable<string> brands = await multi.ReadAsync<string>();
        IEnumerable<string> models = await multi.ReadAsync<string>();
        IEnumerable<int> years = await multi.ReadAsync<int>();
        IEnumerable<string> bodyTypes = await multi.ReadAsync<string>();
        IEnumerable<string> engineSpecs = await multi.ReadAsync<string>();

        return new CarModelSelectionResponse()
        {
            Brands = brands.ToList(),
            Models = models.ToList(),
            Years = years.ToList(),
            BodyTypes = bodyTypes.ToList(),
            EngineSpecs = engineSpecs.ToList(),
        };
    }
}
