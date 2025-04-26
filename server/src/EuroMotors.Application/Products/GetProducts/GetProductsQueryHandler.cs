using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

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
            whereClause += " AND pcm.car_model_id = ANY(@CarModelIds)";
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            whereClause += " AND (LOWER(p.name) LIKE LOWER(@SearchPattern) OR LOWER(p.vendor_code) LIKE LOWER(@SearchPattern))";
        }

        if (request.IsDiscounted == true)
        {
            whereClause += " AND p.discount > 0";
        }

        //if (request.IsNew == true)
        //{
        //    whereClause += " AND p.created_at >= @NewThreshold";
        //}

        //if (request.IsPopular == true)
        //{
        //    var 1:
        //    whereClause += " AND p.view_count >= @PopularThreshold";

        //    var 2: 
        //     whereClause += " AND p.id IN (SELECT product_id FROM order_items GROUP BY product_id HAVING COUNT(*) >= @PopularThreshold)";
        //}

        string countSql = $"""
                           SELECT COUNT(DISTINCT p.id)
                           FROM products p
                           LEFT JOIN product_car_models pcm ON p.id = pcm.product_id
                           {whereClause}
                           """;

        int totalItems = await connection.ExecuteScalarAsync<int>(countSql, new
        {
            request.CategoryIds,
            request.CarModelIds,
            SearchPattern = $"%{request.SearchTerm}%"
        });

        string productIdsSql = $@"
            SELECT p.id
            FROM products p
            LEFT JOIN product_car_models pcm ON p.id = pcm.product_id
            {whereClause}
            GROUP BY p.id
            ORDER BY p.{orderBy}
            LIMIT @Limit OFFSET @Offset
        ";

        IEnumerable<Guid> productIds = await connection.QueryAsync<Guid>(
            productIdsSql,
            new
            {
                request.CategoryIds,
                request.CarModelIds,
                SearchPattern = $"%{request.SearchTerm}%",
                Limit = limit,
                Offset = offset
            }
        );

        if (!productIds.Any())
        {
            return Result.Success(new Pagination<ProductResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalItems,
                Data = new List<ProductResponse>()
            });
        }

        string sql = $@"
            SELECT
                p.id AS {nameof(ProductResponse.Id)},
                p.category_id AS {nameof(ProductResponse.CategoryId)},
                p.name AS {nameof(ProductResponse.Name)},
                p.vendor_code AS {nameof(ProductResponse.VendorCode)},
                p.price AS {nameof(ProductResponse.Price)},
                p.discount AS {nameof(ProductResponse.Discount)},
                p.stock AS {nameof(ProductResponse.Stock)},
                p.is_available AS {nameof(ProductResponse.IsAvailable)},
                p.slug AS {nameof(ProductResponse.Slug)},
                pi.id AS {nameof(ProductImageResponse.ProductImageId)},
                pi.path AS {nameof(ProductImageResponse.Path)},
                pi.product_id AS {nameof(ProductImageResponse.ProductId)},
                s.specification_name AS SpecificationName,
                s.specification_value AS SpecificationValue
            FROM products p
            LEFT JOIN product_images pi ON pi.product_id = p.id
            LEFT JOIN product_specifications s ON s.product_id = p.id
            WHERE p.id = ANY(@ProductIds)
            ORDER BY p.{orderBy}
        ";

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
                    productEntry.CarModelIds = new List<Guid>();
                    productDictionary[product.Id] = productEntry;
                }

                if (image != null && image.ProductImageId != Guid.Empty && !productEntry.Images.Any(i => i.ProductImageId == image.ProductImageId))
                {
                    productEntry.Images.Add(image);
                }

                if (!string.IsNullOrEmpty(specification?.SpecificationName) && !string.IsNullOrEmpty(specification.SpecificationValue) && !productEntry.Specifications.Any(s => s.SpecificationName == specification.SpecificationName && s.SpecificationValue == specification.SpecificationValue))
                {
                    productEntry.Specifications.Add(specification);
                }

                return productEntry;
            },
            new { ProductIds = productIds.ToArray() },
            splitOn: "ProductImageId,SpecificationName"
        );

        string carModelSql = @"
            SELECT 
                product_id,
                car_model_id
            FROM product_car_models
            WHERE product_id = ANY(@ProductIds)
        ";

        IEnumerable<(Guid ProductId, Guid CarModelId)> carModelMappings = await connection.QueryAsync<(Guid ProductId, Guid CarModelId)>(
            carModelSql,
            new { ProductIds = productIds.ToArray() }
        );

        foreach ((Guid ProductId, Guid CarModelId) mapping in carModelMappings)
        {
            if (productDictionary.TryGetValue(mapping.ProductId, out ProductResponse? product))
            {
                product.CarModelIds.Add(mapping.CarModelId);
            }
        }

        var products = productDictionary.Values.ToList();

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
