using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.GetProductsByBrandName;

internal sealed class GetProductsByBrandNameQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
    
    public async Task<Result<Pagination<ProductResponse>>> Handle(GetProductsByBrandNameQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CreateCacheKey(request);
        
        Pagination<ProductResponse>? cachedResult = await cacheService.GetAsync<Pagination<ProductResponse>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result.Success(cachedResult);
        }
        
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        string carModelSqlForBrand = @"
            SELECT cm.id
            FROM car_models cm
            JOIN car_brands cb ON cm.car_brand_id = cb.id
            WHERE LOWER(cb.name) = LOWER(@BrandName)
        ";

        IEnumerable<Guid> carModelIds = await connection.QueryAsync<Guid>(
            carModelSqlForBrand,
            new { request.BrandName }
        );

        var carModelIdsList = carModelIds.ToList();

        if (!carModelIdsList.Any())
        {
            return Result.Failure<Pagination<ProductResponse>>(CarModelErrors.BrandNameNotFound(request.BrandName));
        }

        string sortPriceDirection = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        string orderBy = string.IsNullOrEmpty(request.SortOrder) ? "name ASC" : $"price {sortPriceDirection}";

        int offset = (request.PageNumber - 1) * request.PageSize;
        int limit = request.PageSize;

        string whereClause = "WHERE pcm.car_model_id = ANY(@CarModelIds)";

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            whereClause += " AND (LOWER(p.name) LIKE LOWER(@SearchPattern) OR LOWER(p.vendor_code) LIKE LOWER(@SearchPattern))";
        }

        string countSql = $"""
                          SELECT COUNT(DISTINCT p.id)
                          FROM products p
                          JOIN product_car_models pcm ON p.id = pcm.product_id
                          {whereClause}
                          """;

        int totalItems = await connection.ExecuteScalarAsync<int>(
            countSql,
            new
            {
                CarModelIds = carModelIdsList.ToArray(),
                SearchPattern = $"%{request.SearchTerm}%"
            }
        );

        string productIdsSql = $@"
            SELECT p.id
            FROM products p
            JOIN product_car_models pcm ON p.id = pcm.product_id
            {whereClause}
            GROUP BY p.id
            ORDER BY p.{orderBy}
            LIMIT @Limit OFFSET @Offset
        ";

        IEnumerable<Guid> productIds = await connection.QueryAsync<Guid>(
            productIdsSql,
            new
            {
                CarModelIds = carModelIdsList.ToArray(),
                SearchPattern = $"%{request.SearchTerm}%",
                Limit = limit,
                Offset = offset
            }
        );

        if (!productIds.Any())
        {
            var emptyResult = new Pagination<ProductResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalItems,
                Data = new List<ProductResponse>()
            };
            
            await cacheService.SetAsync(cacheKey, emptyResult, CacheExpiration, cancellationToken);
            
            return Result.Success(emptyResult);
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
        
        await cacheService.SetAsync(cacheKey, paginatedResult, CacheExpiration, cancellationToken);

        return Result.Success(paginatedResult);
    }
    
    private static string CreateCacheKey(GetProductsByBrandNameQuery request)
    {
        string baseKey = CacheKeys.Products.GetByBrandName(request.BrandName);
        
        string searchTerm = !string.IsNullOrEmpty(request.SearchTerm) ? request.SearchTerm : "none";
        string sortOrder = !string.IsNullOrEmpty(request.SortOrder) ? request.SortOrder : "default";
        string pagination = $"{request.PageNumber}_{request.PageSize}";
        
        return $"{baseKey}:{searchTerm}:{sortOrder}:{pagination}";
    }
}
