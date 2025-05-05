using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.GetProductsByCategoryWithChildren;

internal sealed class GetProductsByCategoryWithChildrenQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public async Task<Result<Pagination<ProductResponse>>> Handle(
        GetProductsByCategoryWithChildrenQuery request,
        CancellationToken cancellationToken)
    {
        string cacheKey = CreateCacheKey(request);

        Pagination<ProductResponse>? cachedResult = await cacheService.GetAsync<Pagination<ProductResponse>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result.Success(cachedResult);
        }

        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string getCategoriesQuery = """
            WITH RECURSIVE category_tree AS (
                -- Base case: start with the parent category
                SELECT id, parent_category_id
                FROM categories
                WHERE id = @CategoryId
                
                UNION ALL
                
                -- Recursive case: get all children
                SELECT c.id, c.parent_category_id
                FROM categories c
                INNER JOIN category_tree ct ON ct.id = c.parent_category_id
            )
            SELECT id FROM category_tree
            """;

        var categoryIds = (await connection.QueryAsync<Guid>(getCategoriesQuery, new { request.CategoryId })).ToList();

        if (!categoryIds.Any())
        {
            return Result.Failure<Pagination<ProductResponse>>(CategoryErrors.NotFound(request.CategoryId));
        }

        string whereClause = "WHERE p.category_id = ANY(@CategoryIds)";
        var parameters = new { CategoryIds = categoryIds.ToArray(), SearchPattern = $"%{request.SearchTerm}%" };

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            whereClause += " AND (LOWER(p.name) LIKE LOWER(@SearchPattern) OR LOWER(p.vendor_code) LIKE LOWER(@SearchPattern))";
        }

        string countSql = $"""
            SELECT COUNT(DISTINCT p.id)
            FROM products p
            {whereClause}
            """;

        int totalItems = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        if (totalItems == 0)
        {
            var emptyResult = new Pagination<ProductResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = 0,
                Data = new List<ProductResponse>()
            };

            await cacheService.SetAsync(cacheKey, emptyResult, CacheExpiration, cancellationToken);

            return Result.Success(emptyResult);
        }

        string sortPriceDirection = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        string orderBy = string.IsNullOrEmpty(request.SortOrder) ? "name ASC" : $"price {sortPriceDirection}";

        string productIdsSql = $"""
            SELECT p.id
            FROM products p
            {whereClause}
            ORDER BY p.{orderBy}
            LIMIT @Limit OFFSET @Offset
            """;

        var paginationParams = new
        {
            parameters.CategoryIds,
            parameters.SearchPattern,
            Limit = request.PageSize,
            Offset = (request.PageNumber - 1) * request.PageSize
        };

        IEnumerable<Guid> productIds = await connection.QueryAsync<Guid>(productIdsSql, paginationParams);

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

        const string productsSql = """
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
            WHERE id = ANY(@ProductIds)
            """;

        var productDictionary = new Dictionary<Guid, ProductResponse>();
        IEnumerable<ProductResponse> products = await connection.QueryAsync<ProductResponse>(productsSql, new { ProductIds = productIds.ToArray() });
        foreach (ProductResponse product in products)
        {
            product.Images = new List<ProductImageResponse>();
            product.Specifications = new List<Specification>();
            product.CarModelIds = new List<Guid>();
            productDictionary[product.Id] = product;
        }

        const string imagesSql = """
            SELECT
                id AS ProductImageId,
                path AS Path,
                product_id AS ProductId
            FROM product_images
            WHERE product_id = ANY(@ProductIds)
            """;

        IEnumerable<ProductImageResponse> images = await connection.QueryAsync<ProductImageResponse>(imagesSql, new { ProductIds = productIds.ToArray() });
        foreach (ProductImageResponse image in images)
        {
            if (productDictionary.TryGetValue(image.ProductId, out ProductResponse? product))
            {
                product.Images.Add(image);
            }
        }

        const string specificationsSql = """
            SELECT
                product_id,
                specification_name AS SpecificationName,
                specification_value AS SpecificationValue
            FROM product_specifications
            WHERE product_id = ANY(@ProductIds)
            """;

        IEnumerable<(Guid ProductId, string SpecificationName, string SpecificationValue)> specifications = await connection.QueryAsync<(Guid ProductId, string SpecificationName, string SpecificationValue)>(
            specificationsSql,
            new { ProductIds = productIds.ToArray() }
        );

        foreach ((Guid ProductId, string SpecificationName, string SpecificationValue) spec in specifications)
        {
            if (productDictionary.TryGetValue(spec.ProductId, out ProductResponse? product))
            {
                product.Specifications.Add(new Specification(spec.SpecificationName, spec.SpecificationValue));
            }
        }

        const string carModelsSql = """
            SELECT
                product_id,
                car_model_id
            FROM product_car_models
            WHERE product_id = ANY(@ProductIds)
            """;

        IEnumerable<(Guid ProductId, Guid CarModelId)> carModels = await connection.QueryAsync<(Guid ProductId, Guid CarModelId)>(
            carModelsSql,
            new { ProductIds = productIds.ToArray() }
        );

        foreach ((Guid ProductId, Guid CarModelId) carModel in carModels)
        {
            if (productDictionary.TryGetValue(carModel.ProductId, out ProductResponse? product))
            {
                product.CarModelIds.Add(carModel.CarModelId);
            }
        }

        var result = new Pagination<ProductResponse>
        {
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            Count = totalItems,
            Data = productDictionary.Values.ToList()
        };

        await cacheService.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return Result.Success(result);
    }

    private static string CreateCacheKey(GetProductsByCategoryWithChildrenQuery request)
    {
        string baseKey = CacheKeys.Products.GetByCategory(request.CategoryId);

        string searchTerm = !string.IsNullOrEmpty(request.SearchTerm) ? request.SearchTerm : "none";
        string sortOrder = !string.IsNullOrEmpty(request.SortOrder) ? request.SortOrder : "default";
        string pagination = $"{request.PageNumber}_{request.PageSize}";

        return $"{baseKey}:{searchTerm}:{sortOrder}:{pagination}";
    }
}
