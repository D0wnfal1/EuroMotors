using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetParentCategories;

internal sealed class GetParentCategoriesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetParentCategoriesQuery, List<CategoryResponse>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"""
                            SELECT
                                id AS {nameof(CategoryResponse.Id)},
                                name AS {nameof(CategoryResponse.Name)},
                                is_available AS {nameof(CategoryResponse.IsAvailable)},
                                image_path AS {nameof(CategoryResponse.ImagePath)},
                                parent_category_id AS {nameof(CategoryResponse.ParentCategoryId)},
                                slug AS {nameof(CategoryResponse.Slug)}
                            FROM categories
                            WHERE parent_category_id IS NULL
                        """);

        List<CategoryResponse> categories = (await connection.QueryAsync<CategoryResponse>(sql.ToString())).AsList();

        return Result.Success(categories);
    }
}

