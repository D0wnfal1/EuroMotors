namespace EuroMotors.Application.Abstractions.Pagination;

public sealed class Pagination<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IReadOnlyCollection<T> Data { get; set; } = [];
}

