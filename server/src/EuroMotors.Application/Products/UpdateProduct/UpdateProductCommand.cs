using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(Guid ProductId, string Name, List<Specification> Specifications, string VendorCode, Guid CategoryId, decimal Price, decimal Discount, int Stock) : ICommand;
