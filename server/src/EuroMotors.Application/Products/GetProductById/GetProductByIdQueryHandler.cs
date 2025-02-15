using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.GetProductById;

internal sealed class GetProductByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(ProductResponse.Id)},
                 category_id AS {nameof(ProductResponse.CategoryId)},
                 car_model_id AS {nameof(ProductResponse.CarModelId)},
                 name AS {nameof(ProductResponse.Name)},
                 description AS {nameof(ProductResponse.Description)},
                 vendor_code AS {nameof(ProductResponse.VendorCode)},
                 price AS {nameof(ProductResponse.Price)},
                 discount AS {nameof(ProductResponse.Discount)},
                 stock AS {nameof(ProductResponse.Stock)},
                 is_available AS {nameof(ProductResponse.IsAvailable)}
             FROM products
             WHERE id = @ProductId
             """;

        ProductResponse? product = await connection.QuerySingleOrDefaultAsync<ProductResponse>(sql, request);

        return product ?? Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
    }
}
