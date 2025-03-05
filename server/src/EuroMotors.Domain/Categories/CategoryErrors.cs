using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound("Category.NotFound", $"The category with the identifier {categoryId} was not found");

    public static readonly Error AlreadyArchived = Error.Problem(
        "Category.AlreadyArchived",
        "The category was already archived");

    public static Error InvalidUrl(Uri url) =>
        Error.Failure("Category.InvalidUrl", $"The URL '{url}' provided for the product image is invalid.");
}
