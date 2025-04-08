using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound("Category.NotFound", $"The category with the identifier {categoryId} was not found");

    public static readonly Error AlreadyArchived = Error.Problem(
        "Category.AlreadyArchived",
        "The category was already archived");

    public static Error InvalidPath(string path) =>
        Error.Failure("Category.InvalidPath", $"The URL '{path}' provided for the product image is invalid.");

    public static Error CannotCreateSubcategoryForSubcategory() =>
        Error.Failure("Category.CannotCreateSubcategoryForSubcategory",
        "Subcategories cannot be created for a category that is already a subcategory.");

    public static Error SubcategoryAlreadyExists() =>
        Error.Failure("Category.SubcategoryAlreadyExists",
        "This subcategory already exists.");


}
