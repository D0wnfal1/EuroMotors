using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Products.SearchProducts;

internal sealed class SearchProductsByCarModelIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchProductsQuery, IReadOnlyCollection<ProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductResponse>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        string sortPriceDirection = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        string orderBy = string.IsNullOrEmpty(request.SortOrder) ? "name ASC" : $"price {sortPriceDirection}";

        int offset = (request.PageNumber - 1) * request.PageSize;
        int limit = request.PageSize;

        string sql = $"""
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
                         LEFT JOIN categories c ON p.category_id = c.id
                         LEFT JOIN car_models cm ON p.car_model_id = cm.id
                         WHERE (@CategoryName IS NULL OR LOWER(c.name) LIKE LOWER(@CategoryName))  
                           AND (@CarModelBrand IS NULL OR LOWER(cm.brand) LIKE LOWER(@CarModelBrand))  
                           AND (@CarModelModel IS NULL OR LOWER(cm.model) LIKE LOWER(@CarModelModel))  
                           AND (@SearchTerm IS NULL OR (LOWER(p.name) LIKE LOWER(@SearchPattern) OR LOWER(p.description) LIKE LOWER(@SearchPattern) OR LOWER(p.vendor_code) LIKE LOWER(@SearchPattern)))
                         ORDER BY {orderBy}
                         LIMIT @Limit OFFSET @Offset
                     """;

        var parameters = new
        {
            request.CategoryName,
            request.CarModelBrand,
            request.CarModelModel,
            request.SearchTerm,
            SearchPattern = $"%{request.SearchTerm}%",
            Limit = limit,
            Offset = offset
        };

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, parameters)).AsList();

        return Result.Success<IReadOnlyCollection<ProductResponse>>(products);
    }

}
