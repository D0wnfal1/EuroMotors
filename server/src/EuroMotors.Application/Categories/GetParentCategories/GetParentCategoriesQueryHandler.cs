using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetParentCategories;

internal sealed class GetParentCategoriesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetParentCategoriesQuery, Pagination<CategoryResponse>>
{
    public async Task<Result<Pagination<CategoryResponse>>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
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

        var parameters = new Dictionary<string, object>();

        if (request.PageSize > 0)
        {
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            sql.AppendLine("LIMIT @PageSize OFFSET @Offset");
        }

        List<CategoryResponse> categories = (await connection.QueryAsync<CategoryResponse>(sql.ToString(), parameters)).AsList();

        var countSql = new StringBuilder();
        countSql.Append("SELECT COUNT(*) FROM categories WHERE parent_category_id IS NULL");

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString());

        var paginatedResult = new Pagination<CategoryResponse>
        {
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            Count = totalCount,
            Data = categories
        };

        return Result.Success(paginatedResult);
    }
}

