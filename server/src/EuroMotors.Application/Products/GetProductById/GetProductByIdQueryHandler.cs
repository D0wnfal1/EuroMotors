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
                 p.id AS {nameof(ProductResponse.Id)},
                 p.category_id AS {nameof(ProductResponse.CategoryId)},
                 p.car_model_id AS {nameof(ProductResponse.CarModelId)},
                 p.name AS {nameof(ProductResponse.Name)},
                 p.description AS {nameof(ProductResponse.Description)},
                 p.vendor_code AS {nameof(ProductResponse.VendorCode)},
                 p.price AS {nameof(ProductResponse.Price)},
                 p.discount AS {nameof(ProductResponse.Discount)},
                 p.stock AS {nameof(ProductResponse.Stock)},
                 p.is_available AS {nameof(ProductResponse.IsAvailable)},
                 COALESCE(array_agg(pi.path) FILTER (WHERE pi.path IS NOT NULL), ARRAY[]::text[]) AS {nameof(ProductResponse.Images)}
             FROM products p
             LEFT JOIN product_images pi ON pi.product_id = p.id
             WHERE p.id = @ProductId
             GROUP BY p.id
             """;

        ProductResponse? product = await connection.QuerySingleOrDefaultAsync<ProductResponse>(sql, request);

        return product ?? Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
    }
}
