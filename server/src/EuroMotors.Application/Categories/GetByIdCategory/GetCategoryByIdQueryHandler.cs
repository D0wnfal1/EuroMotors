using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;

namespace EuroMotors.Application.Categories.GetByIdCategory;

internal sealed class GetCategoryByIdQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);
    
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.Categories.GetById(request.CategoryId);
        
        CategoryResponse? cachedCategory = await cacheService.GetAsync<CategoryResponse>(cacheKey, cancellationToken);
        if (cachedCategory != null)
        {
            return Result.Success(cachedCategory);
        }
        
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(CategoryResponse.Id)},
                 name AS {nameof(CategoryResponse.Name)},
                 image_path AS {nameof(CategoryResponse.ImagePath)},
                 parent_category_id AS {nameof(CategoryResponse.ParentCategoryId)},
                 slug AS {nameof(CategoryResponse.Slug)}
             FROM categories
             WHERE id = @CategoryId
             """;

        CategoryResponse? category = await connection.QuerySingleOrDefaultAsync<CategoryResponse>(sql, request);

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound(request.CategoryId));
        }
        
        await cacheService.SetAsync(cacheKey, category, CacheExpiration, cancellationToken);

        return Result.Success(category);
    }
}
