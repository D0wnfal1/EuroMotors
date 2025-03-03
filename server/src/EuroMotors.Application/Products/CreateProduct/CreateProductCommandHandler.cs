
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICarModelRepository carModelRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure<Guid>(ProductErrors.ProductCategoryNotFound(request.CategoryId));
        }

        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure<Guid>(ProductErrors.ProductCarModelNotFound(request.CarModelId));
        }

        Result<Product> result = Product.Create(
            request.Name,
            request.Description,
            request.VendorCode,
            request.CategoryId,
            request.CarModelId,
            request.Price,
            request.Discount,
            request.Stock,
            request.IsAvailable);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        productRepository.Insert(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
