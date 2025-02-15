using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;

namespace EuroMotors.Application.ProductImages.GetProductImageById;

internal sealed class GetProductImageByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetProductImageByIdQuery, ProductImageResponse>
{
    public async Task<Result<ProductImageResponse>> Handle(GetProductImageByIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(ProductImageResponse.Id)},
                 url AS {nameof(ProductImageResponse.Url)},
                 product_id AS {nameof(ProductImageResponse.ProductId)}
             FROM product_images
             WHERE id = @ProductImageId
             """;

        ProductImageResponse? productImage = await connection.QuerySingleOrDefaultAsync<ProductImageResponse>(sql, request);

        if (productImage is null)
        {
            return Result.Failure<ProductImageResponse>(ProductImageErrors.ProductImageNotFound(request.ProductImageId));
        }

        return productImage;
    }
}
