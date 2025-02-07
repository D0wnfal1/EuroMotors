using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Todos.Events;

namespace EuroMotors.Domain.Todos;

public sealed class TodoItem : Entity
{
    private TodoItem(Guid id, Guid userId, string description, DateTime? dueDate, bool isCompleted, DateTime createdAt, DateTime? completedAt, Priority priority) : base(id)
    {
        UserId = userId;
        Description = description;
        DueDate = dueDate;
        IsCompleted = isCompleted;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        Priority = priority;
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
        var todoItem = new TodoItem(Guid.NewGuid(), userId, description, null, isComplete, createdAt, null, priority);

        todoItem.RaiseDomainEvents(new TodoItemCreatedDomainEvent(todoItem.Id));
        
        return todoItem;
    }

}
