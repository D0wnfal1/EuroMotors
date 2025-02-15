using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.SearchProductsByCarModelId;

internal sealed class SearchProductsByCarModelIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchProductsByCarModelIdQuery, IReadOnlyCollection<ProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductResponse>>> Handle(
        SearchProductsByCarModelIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 p.id AS {nameof(ProductResponse.Id)},
                 p.category_id AS {nameof(ProductResponse.CategoryId)},
                 p.car_model_id AS {nameof(ProductResponse.CarModelId)},
                 p.name AS {nameof(ProductResponse.Name)},
                 p.description AS {nameof(ProductResponse.Description)},
                 p.vendor_code AS {nameof(ProductResponse.VendorCode)},
                 p.price AS {nameof(ProductResponse.Price)},
                 p.discount AS {nameof(ProductResponse.Discount)},
                 p.stock AS {nameof(ProductResponse.Stock)},
                 p.is_available AS {nameof(ProductResponse.IsAvailable)}
             FROM products p
             INNER JOIN car_models cm ON cm.id = p.car_model_id
             WHERE cm.id = @CarModelId
             """;

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, request)).AsList();

        if (products.Count == 0)
        {
            return Result.Failure<IReadOnlyCollection<ProductResponse>>(ProductErrors.ProductsNotFoundForCarModel(request.CarModelId));
        }

        return products;
    }
}
