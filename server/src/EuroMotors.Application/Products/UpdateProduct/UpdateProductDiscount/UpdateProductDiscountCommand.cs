using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductDiscount;

public record UpdateProductDiscountCommand(Guid ProductId, decimal Discount) : ICommand;
