using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.CreateProduct;

public sealed record CreateProductCommand(string Name, List<Specification> Specifications, string VendorCode,
    Guid CategoryId, List<Guid> CarModelIds,
    decimal Price, decimal Discount,
    int Stock) : ICommand<Guid>;
