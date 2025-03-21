namespace EuroMotors.Application.Products.GetProducts;

public sealed class Pagination<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IReadOnlyCollection<T> Data { get; set; } = [];
}

