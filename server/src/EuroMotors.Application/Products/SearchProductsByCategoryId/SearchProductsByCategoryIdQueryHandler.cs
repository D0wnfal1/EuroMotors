using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;
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
             WHERE category_id = @CategoryId
             """;

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, request)).AsList();

        return products.Count == 0 ? Result.Failure<IReadOnlyCollection<ProductResponse>>(ProductErrors.ProductCategoryNotFound(request.CategoryId)) : products;
    }
}
