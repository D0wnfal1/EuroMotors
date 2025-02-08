using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Category.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Category;

public class Category : Entity
{
    private Category()
    {

    }

    public string Name { get; private set; }

    public bool IsArchived { get; private set; }

    public List<Product> Products { get; private set; } = new();

    public static Category Create(string name, bool isArchived)
    {
        var category = new Category()
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsArchived = isArchived
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
}
