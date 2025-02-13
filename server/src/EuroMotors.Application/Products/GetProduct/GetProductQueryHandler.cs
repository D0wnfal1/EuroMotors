using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.GetProduct;

internal sealed class GetProductQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetProductQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

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
