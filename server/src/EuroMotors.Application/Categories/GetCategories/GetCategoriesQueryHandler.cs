using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCategoriesQuery, Pagination<CategoryResponse>>
{
    public async Task<Result<Pagination<CategoryResponse>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine(
            $"""
             SELECT
                 id AS {nameof(CategoryResponse.Id)},
                 name AS {nameof(CategoryResponse.Name)},
                 is_archived AS {nameof(CategoryResponse.IsArchived)},
                 image_path AS {nameof(CategoryResponse.ImagePath)},
                 parent_category_id AS {nameof(CategoryResponse.ParentCategoryId)},
                 slug AS {nameof(CategoryResponse.Slug)}
             FROM categories
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
        countSql.Append("SELECT COUNT(*) FROM categories ");

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString());

        if (request.PageSize > 0)
        {
            var paginatedResult = new Pagination<CategoryResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = categories
            };

            return Result.Success(paginatedResult);
        }

        var result = new Pagination<CategoryResponse>
        {
            PageIndex = 1,
            PageSize = totalCount,
            Count = totalCount,
            Data = categories
        };

        return Result.Success(result);
    }
}
