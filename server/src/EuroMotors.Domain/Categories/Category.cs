using System;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories.Events;
using EuroMotors.Domain.ProductImages.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Categories;

public class Category : Entity
{
    private Category()
    {

    }

    public string Name { get; private set; }

    public bool IsArchived { get; private set; }

    public Uri? ImageUrl { get; private set; }

    public List<Product> Products { get; private set; } = [];

    public static Category Create(string name)
    {
        var category = new Category()
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsArchived = false
        };

        category.RaiseDomainEvent(new CategoryCreatedDomainEvent(category.Id));

        return category;
    }

    public void Archive()
    {
        if (IsArchived)
        {
            return;
        }

        IsArchived = true;

        RaiseDomainEvent(new CategoryArchivedDomainEvent(Id));
    }

    public void ChangeName(string name)
    {
        if (Name == name)
        {
            return;
        }

        Name = name;

        RaiseDomainEvent(new CategoryNameChangedDomainEvent(Id, Name));
    }

    public Result UpdateImage(Uri newUrl)
    {
        if (string.IsNullOrWhiteSpace(newUrl.ToString()))
        {
            return Result.Failure(CategoryErrors.InvalidUrl(newUrl));
        }

        ImageUrl = newUrl;

        RaiseDomainEvent(new CategoryImageUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result DeleteImage()
    {
        if (Id == Guid.Empty)
        {
            return Result.Failure(CategoryErrors.NotFound(Id));
        }

        ImageUrl = null;

        RaiseDomainEvent(new CategoryImageDeletedDomainEvent(Id));

        return Result.Success();
    }
}
