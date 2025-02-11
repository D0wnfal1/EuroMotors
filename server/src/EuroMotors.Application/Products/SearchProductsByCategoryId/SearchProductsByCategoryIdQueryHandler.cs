using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.SearchProductsByCategoryId;

internal sealed class SearchProductsByCategoryIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchProductsByCategoryIdQuery, IReadOnlyCollection<ProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductResponse>>> Handle(
        SearchProductsByCategoryIdQuery request,
        CancellationToken cancellationToken)
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
             WHERE category_id = @CategoryId
             """;

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, request)).AsList();

        if (products.Count == 0)
        {
            return Result.Failure<IReadOnlyCollection<ProductResponse>>(ProductErrors.ProductsNotFoundForCategory(request.CategoryId));
        }

        return products;
    }
}
