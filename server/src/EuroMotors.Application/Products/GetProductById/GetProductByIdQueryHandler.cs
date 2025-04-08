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
                  p.slug AS {nameof(ProductResponse.Slug)},
                  pi.id AS {nameof(ProductImageResponse.ProductImageId)},
                  pi.path AS {nameof(ProductImageResponse.Path)},
                  pi.product_id AS {nameof(ProductImageResponse.ProductId)}
              FROM products p
              LEFT JOIN product_images pi ON pi.product_id = p.id
              WHERE p.id = @ProductId
              """;

        var productDictionary = new Dictionary<Guid, ProductResponse>();

        await connection.QueryAsync<ProductResponse, ProductImageResponse, ProductResponse>(
            sql,
            (product, image) =>
            {
                if (!productDictionary.TryGetValue(product.Id, out ProductResponse? productEntry))
                {
                    productEntry = product;
                    productEntry.Images = [];
                    productDictionary.Add(productEntry.Id, productEntry);
                }
                if (image != null && image.ProductImageId != Guid.Empty)
                {
                    productEntry.Images.Add(image);
                }
                return productEntry;
            },
            new { request.ProductId },
            splitOn: "ProductImageId"
        );

        if (!productDictionary.TryGetValue(request.ProductId, out ProductResponse? productResult))
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound(request.ProductId));
        }

        return Result.Success(productResult);
    }
}
