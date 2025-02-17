using EuroMotors.Domain.Todos;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Todos;

public sealed class CreateTodoRequest
{
    [JsonRequired]
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    [JsonRequired]
    public Priority Priority { get; set; }
}

