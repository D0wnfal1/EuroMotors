using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImages.GetProductImage;
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
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(ProductImageResponse.Id)},
                 url AS {nameof(ProductImageResponse.Url)},
                 product_id AS {nameof(ProductImageResponse.ProductId)}
             FROM product_images
             WHERE product_id = @ProductId
             """;

        List<ProductImageResponse> productImages = (await connection.QueryAsync<ProductImageResponse>(sql, request)).AsList();

        if (productImages.Count == 0)
        {
            return Result.Failure<IReadOnlyCollection<ProductImageResponse>>(ProductImageErrors.ProductImagesNotFound(request.ProductId));
        }

        return productImages;
    }
}
