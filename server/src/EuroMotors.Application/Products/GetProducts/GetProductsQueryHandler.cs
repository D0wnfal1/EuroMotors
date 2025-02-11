using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Products.GetProducts;

public class GetProductsQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetProductsQuery, IReadOnlyCollection<ProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
              SELECT
                 id As {nameof(ProductResponse.Id)},
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

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, request)).AsList();

        return products;
    }
}
