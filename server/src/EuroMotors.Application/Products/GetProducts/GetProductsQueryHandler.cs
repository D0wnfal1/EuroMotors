using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.SearchProducts;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetProductsQuery, Pagination<ProductResponse>>
{
    public async Task<Result<Pagination<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        string sortPriceDirection = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        string orderBy = string.IsNullOrEmpty(request.SortOrder) ? "name ASC" : $"price {sortPriceDirection}";

        int offset = (request.PageNumber - 1) * request.PageSize;
        int limit = request.PageSize;

        string whereClause = "WHERE (1=1)";

        if (request.CategoryIds?.Any() == true)
        {
            whereClause += " AND p.category_id = ANY(@CategoryIds)";
        }
        if (request.CarModelIds?.Any() == true)
        {
            whereClause += " AND p.car_model_id = ANY(@CarModelIds)";
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            whereClause += " AND (LOWER(p.name) LIKE LOWER(@SearchPattern) OR LOWER(p.description) LIKE LOWER(@SearchPattern) OR LOWER(p.vendor_code) LIKE LOWER(@SearchPattern))";
        }

        string countSql = $"""
        SELECT COUNT(*)
        FROM products p
        {whereClause}
        """;

        int totalItems = await connection.ExecuteScalarAsync<int>(countSql, new
        {
            request.CategoryIds,
            request.CarModelIds,
            SearchPattern = $"%{request.SearchTerm}%"
        });

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
                     {whereClause}
                     ORDER BY {orderBy}
                     LIMIT @Limit OFFSET @Offset
                 """;

        var parameters = new
        {
            request.CategoryIds,
            request.CarModelIds,
            SearchPattern = $"%{request.SearchTerm}%",
            Limit = limit,
            Offset = offset
        };

        List<ProductResponse> products = (await connection.QueryAsync<ProductResponse>(sql, parameters)).AsList();

        var paginatedResult = new Pagination<ProductResponse>
        {
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            Count = totalItems,
            Data = products
        };

        return Result.Success(paginatedResult);
    }
}
