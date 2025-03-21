using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImages.GetProductImageById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.GetProductImagesByProductId;

internal sealed class GetProductImagesByProductIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetProductImagesByProductIdQuery, IReadOnlyCollection<ProductImageResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProductImageResponse>>> Handle(
        GetProductImagesByProductIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 id AS Id,
                 url AS Url,
                 product_id AS ProductId
             FROM product_images
             WHERE product_id = @ProductId
             """;

        var productImages = (await connection.QueryAsync<ProductImageResponse>(sql, new { request.ProductId }))
            .Select(img => new ProductImageResponse(img.Id, img.Url, img.ProductId))
            .ToList();

        if (!productImages.Any())
        {
            return Result.Failure<IReadOnlyCollection<ProductImageResponse>>(
                ProductImageErrors.ProductImagesNotFound(request.ProductId));
        }

        return Result.Success<IReadOnlyCollection<ProductImageResponse>>(productImages);
    }
}
