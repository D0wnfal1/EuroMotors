using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Category.Events;

namespace EuroMotors.Domain.Category;

public class Category : Entity
{
    private Category(Guid id,
        string name,
        bool isArchived)
        : base(id)
    {
        Name = name;
        IsArchived = isArchived;
    }

    public string Name { get; private set; }

    public bool IsArchived { get; private set; }

    public static Category Create(string name, bool isArchived)
    {
        var category = new Category(Guid.NewGuid(), name, isArchived);

        category.RaiseDomainEvents(new CategoryCreatedDomainEvent(category.Id));

        return category;
    }
}
