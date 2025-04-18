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

        const string sql = """
            SELECT
                p.id AS Id,
                p.category_id AS CategoryId,
                p.car_model_id AS CarModelId,
                p.name AS Name,
                p.vendor_code AS VendorCode,
                p.price AS Price,
                p.discount AS Discount,
                p.stock AS Stock,
                p.is_available AS IsAvailable,
                p.slug AS Slug,

                pi.id AS ProductImageId,
                pi.path AS Path,
                pi.product_id AS ProductId,

                s.specification_name AS SpecificationName,
                s.specification_value AS SpecificationValue
            FROM products p
            LEFT JOIN product_images pi ON pi.product_id = p.id
            LEFT JOIN product_specifications s ON s.product_id = p.id
            WHERE p.id = @ProductId
        """;

        var productDictionary = new Dictionary<Guid, ProductResponse>();

        await connection.QueryAsync<ProductResponse, ProductImageResponse, Specification, ProductResponse>(
            sql,
            (product, image, specification) =>
            {
                if (!productDictionary.TryGetValue(product.Id, out ProductResponse? productEntry))
                {
                    productEntry = product;
                    productEntry.Images = new List<ProductImageResponse>();
                    productEntry.Specifications = new List<Specification>();
                    productDictionary.Add(productEntry.Id, productEntry);
                }

                if (image != null && image.ProductImageId != Guid.Empty)
                {
                    productEntry.Images.Add(image);
                }

                if (specification?.SpecificationName != null && specification.SpecificationValue != null)
                {
                    productEntry.Specifications.Add(specification);
                }

                return productEntry;
            },
            new { request.ProductId },
            splitOn: "ProductImageId,SpecificationName"
        );

        if (!productDictionary.TryGetValue(request.ProductId, out ProductResponse? productResult))
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
        }

        return Result.Success(productResult);
    }
}
