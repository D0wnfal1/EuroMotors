﻿using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem()
    {

    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Price { get; private set; }

    public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        return new OrderItem()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Price = quantity * unitPrice
        };
    }
}
