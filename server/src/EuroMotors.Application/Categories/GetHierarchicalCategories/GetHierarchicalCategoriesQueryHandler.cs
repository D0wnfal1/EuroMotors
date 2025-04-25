using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Categories.GetHierarchicalCategories;

internal sealed class GetHierarchicalCategoriesQueryHandler(
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>>
{
    public async Task<Result<Pagination<HierarchicalCategoryResponse>>> Handle(
        GetHierarchicalCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string countSql = @"
            SELECT COUNT(*) 
            FROM categories 
            WHERE parent_category_id IS NULL";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql);

        var parentSql = new StringBuilder(@"
            SELECT
                id      AS Id,
                name    AS Name,
                is_available AS IsAvailable,
                image_path   AS ImagePath,
                slug    AS Slug
            FROM categories
            WHERE parent_category_id IS NULL
            ORDER BY name
        ");

        var parentParams = new DynamicParameters();
        if (request.PageSize > 0 && request.PageNumber > 0)
        {
            parentParams.Add("PageSize", request.PageSize);
            parentParams.Add("Offset", (request.PageNumber - 1) * request.PageSize);
            parentSql.AppendLine("LIMIT @PageSize OFFSET @Offset");
        }

        var parentCategories = (await connection
            .QueryAsync<CategoryWithParentResponse>(parentSql.ToString(), parentParams))
            .ToList();

        if (!parentCategories.Any())
        {
            return Result.Success(new Pagination<HierarchicalCategoryResponse>
            {
                PageIndex = request.PageNumber,
                PageSize = request.PageSize,
                Count = totalCount,
                Data = new List<HierarchicalCategoryResponse>()
            });
        }

        Guid[] parentIds = parentCategories.Select(p => p.Id).ToArray();
        const string childSql = @"
            SELECT
                id                    AS Id,
                name                  AS Name,
                is_available          AS IsAvailable,
                image_path            AS ImagePath,
                parent_category_id    AS ParentCategoryId,
                slug                  AS Slug
            FROM categories
            WHERE parent_category_id = ANY(@ParentIds)
            ORDER BY name";
        var childCategories = (await connection
            .QueryAsync<CategoryWithParentResponse>(childSql, new { ParentIds = parentIds }))
            .ToList();

        var resultData = parentCategories
            .Select(parent => new HierarchicalCategoryResponse
            {
                Id = parent.Id,
                Name = parent.Name,
                IsAvailable = parent.IsAvailable,
                ImagePath = parent.ImagePath,
                Slug = parent.Slug,
                SubCategories = childCategories
                    .Where(child => child.ParentCategoryId == parent.Id)
                    .Select(child => new HierarchicalCategoryResponse
                    {
                        Id = child.Id,
                        Name = child.Name,
                        IsAvailable = child.IsAvailable,
                        ImagePath = child.ImagePath,
                        Slug = child.Slug
                    })
                    .ToList()
            })
            .ToList();

        var pagination = new Pagination<HierarchicalCategoryResponse>
        {
            PageIndex = request.PageNumber > 0 ? request.PageNumber : 1,
            PageSize = request.PageSize > 0 ? request.PageSize : totalCount,
            Count = totalCount,
            Data = resultData
        };

        return Result.Success(pagination);
    }
}

