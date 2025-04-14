using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Categories;

public class Category : Entity
{
    private Category()
    {

    }

    public string Name { get; private set; }

    public bool IsAvailable { get; private set; }

    public string? ImagePath { get; private set; }

    public Guid? ParentCategoryId { get; private set; }

    public Category? ParentCategory { get; private set; }

    private readonly List<Category> _subcategories = [];

    public Slug Slug { get; private set; }

    public IReadOnlyCollection<Category> Subcategories => _subcategories.AsReadOnly();


    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();


    public static Category Create(string name, Guid? parentCategoryId = null)
    {
        var category = new Category()
        {
            Id = Guid.NewGuid(),
            Name = name,

            IsAvailable = true,
            ParentCategoryId = parentCategoryId
        };

        category.Slug = category.GenerateSlug();

        category.RaiseDomainEvent(new CategoryCreatedDomainEvent(category.Id));

        return category;
    }

    public Result AddSubcategory(Category subcategory)
    {
        if (ParentCategoryId != null)
        {
            return Result.Failure(CategoryErrors.CannotCreateSubcategoryForSubcategory());
        }

        if (_subcategories.Contains(subcategory))
        {
            return Result.Failure(CategoryErrors.SubcategoryAlreadyExists());
        }

        _subcategories.Add(subcategory);
        subcategory.SetParent(this);

        subcategory.Slug = subcategory.GenerateSlug();
        return Result.Success();
    }

    public void SetParent(Category parent)
    {
        ParentCategoryId = parent.Id;
        ParentCategory = parent;
    }

    public void SetAvailability(bool isAvailable)
    {
        if (IsAvailable == isAvailable)
        {
            return;
        }

        IsAvailable = isAvailable;

        if (isAvailable)
        {
            RaiseDomainEvent(new CategoryIsAvailableDomainEvent(Id));
        }
        else
        {
            RaiseDomainEvent(new CategoryIsNotAvailableDomainEvent(Id));
        }
    }

    public void ChangeName(string name)
    {
        if (Name == name)
        {
            return;
        }

        Name = name;
        Slug = GenerateSlug();

        RaiseDomainEvent(new CategoryNameChangedDomainEvent(Id, Name));
    }

    public Result SetImagePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure(CategoryErrors.InvalidPath(path));
        }
        ImagePath = path;

        RaiseDomainEvent(new CategoryImageUpdatedDomainEvent(Id));

        return Result.Success();    
    }

    private Slug GenerateSlug()
    {
        return Slug.GenerateSlug(ParentCategory != null ? $"{ParentCategory.Slug.Value}/{Name}" :
            Name);
    }
}
