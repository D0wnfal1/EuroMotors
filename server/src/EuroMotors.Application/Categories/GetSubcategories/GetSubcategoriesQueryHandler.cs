using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetSubcategories;

internal sealed class GetSubcategoriesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetSubcategoriesQuery, List<CategoryResponse>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetSubcategoriesQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine("""
                           SELECT
                               id AS Id,
                               name AS Name,
                               is_archived AS IsArchived,
                               image_path AS ImagePath,
                               parent_category_id AS ParentCategoryId,
                               slug AS Slug
                           FROM categories
                           WHERE parent_category_id = @ParentCategoryId
                       """);

        var parameters = new Dictionary<string, object>
        {
            { "ParentCategoryId", request.ParentCategoryId }
        };

        List<CategoryResponse> categories = (await connection.QueryAsync<CategoryResponse>(sql.ToString(), parameters)).AsList();

        return Result.Success(categories);
    }
}

