using EuroMotors.Domain.Todos;
using EuroMotors.Domain.Todos.Events;
using EuroMotors.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
