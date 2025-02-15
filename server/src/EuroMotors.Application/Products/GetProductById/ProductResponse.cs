namespace EuroMotors.Application.Products.GetProductById;

public sealed record ProductResponse(
    Guid Id,
    Guid CategoryId,
    Guid CarModelId,
    string Name,
    string Description,
    string VendorCode,
    decimal Price,
    decimal Discount,
    int Stock,
    bool IsAvailable
);
