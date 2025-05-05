using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICarModelRepository carModelRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure<Guid>(CategoryErrors.NotFound(request.CategoryId));
        }

        List<CarModel> carModels = [];
        foreach (Guid carModelId in request.CarModelIds)
        {
            CarModel? carModel = await carModelRepository.GetByIdAsync(carModelId, cancellationToken);

            if (carModel is null)
            {
                return Result.Failure<Guid>(CarModelErrors.ModelNotFound(carModelId));
            }

            carModels.Add(carModel);
        }

        IEnumerable<(string SpecificationName, string SpecificationValue)> specs = request.Specifications
            .Select(s => (s.SpecificationName, s.SpecificationValue));

        Result<Product> result = Product.Create(
            request.Name,
            specs,
            request.VendorCode,
            request.CategoryId,
            carModels,
            request.Price,
            request.Discount,
            request.Stock);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        productRepository.Insert(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(result.Value, cancellationToken);

        return result.Value.Id;
    }

    private async Task InvalidateCacheAsync(Product product, CancellationToken cancellationToken)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);

        await cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetByCategory(product.CategoryId), cancellationToken);

    }
}
