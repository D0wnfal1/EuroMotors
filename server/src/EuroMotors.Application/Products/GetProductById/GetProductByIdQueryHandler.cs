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

        const string productSql = """
            SELECT
                id AS Id,
                category_id AS CategoryId,
                name AS Name,
                vendor_code AS VendorCode,
                price AS Price,
                discount AS Discount,
                stock AS Stock,
                is_available AS IsAvailable,
                slug AS Slug
            FROM products
            WHERE id = @ProductId
        """;

        ProductResponse? product = await connection.QueryFirstOrDefaultAsync<ProductResponse>(
            productSql,
            new { request.ProductId }
        );

        if (product == null)
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
        }

        // Get car model IDs for the product
        const string carModelsSql = """
            SELECT
                car_model_id AS CarModelId
            FROM product_car_models
            WHERE product_id = @ProductId
        """;

        IEnumerable<Guid> carModelIds = await connection.QueryAsync<Guid>(
            carModelsSql,
            new { request.ProductId },
            commandType: CommandType.Text
        );
        product.CarModelIds = carModelIds.ToList();

        const string imagesSql = """
            SELECT
                id AS ProductImageId,
                path AS Path,
                product_id AS ProductId
            FROM product_images
            WHERE product_id = @ProductId
        """;

        IEnumerable<ProductImageResponse> images = await connection.QueryAsync<ProductImageResponse>(
            imagesSql,
            new { request.ProductId }
        );
        product.Images = images.ToList();

        const string specificationsSql = """
            SELECT
                specification_name AS SpecificationName,
                specification_value AS SpecificationValue
            FROM product_specifications
            WHERE product_id = @ProductId
        """;

        IEnumerable<Specification> specifications = await connection.QueryAsync<Specification>(
            specificationsSql,
            new { request.ProductId }
        );
        product.Specifications = specifications.ToList();

        return Result.Success(product);
    }
}
