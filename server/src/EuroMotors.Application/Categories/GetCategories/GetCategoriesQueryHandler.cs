using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);
    
    public async Task<Result<List<CategoryResponse>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        // Получаем ключ кеша
        string cacheKey = CacheKeys.Categories.GetList();
        
        // Проверяем кеш
        var cachedCategories = await cacheService.GetAsync<List<CategoryResponse>>(cacheKey, cancellationToken);
        if (cachedCategories != null)
        {
            return Result.Success(cachedCategories);
        }
        
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine(
            $"""
             SELECT
                 id AS {nameof(CategoryResponse.Id)},
                 name AS {nameof(CategoryResponse.Name)},
                 image_path AS {nameof(CategoryResponse.ImagePath)},
                 parent_category_id AS {nameof(CategoryResponse.ParentCategoryId)},
                 slug AS {nameof(CategoryResponse.Slug)}
             FROM categories
             """);

        var parameters = new Dictionary<string, object>();

        List<CategoryResponse> categories = (await connection.QueryAsync<CategoryResponse>(sql.ToString(), parameters)).AsList();
        
        // Кешируем результат
        await cacheService.SetAsync(cacheKey, categories, CacheExpiration, cancellationToken);

        return Result.Success(categories);
    }
}
