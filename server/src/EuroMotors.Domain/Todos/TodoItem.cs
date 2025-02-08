using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Todos.Events;

namespace EuroMotors.Domain.Todos;

public sealed class TodoItem : Entity
{
    private TodoItem()
    {

    }

    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Priority Priority { get; set; }

    public static TodoItem Create(Guid userId, string description, List<string> labels, bool isComplete, DateTime createdAt, Priority priority)
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Description = description,
            Labels = labels,
            IsCompleted = isComplete,
            CreatedAt = createdAt,
            Priority = priority
        };

        todoItem.RaiseDomainEvent(new TodoItemCreatedDomainEvent(todoItem.Id));
        
        return todoItem;
    }

}
