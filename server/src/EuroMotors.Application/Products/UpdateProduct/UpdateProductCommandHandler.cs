using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICarModelRepository carModelRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.CategoryId));
        }

        CarModel? carModel = await carModelRepository.GetByIdAsync(request.CarModelId, cancellationToken);

        if (carModel is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.CarModelId));
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Discount,
            request.Stock);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
