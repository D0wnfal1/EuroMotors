namespace EuroMotors.Domain.Orders;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Paid = 2,
    Shipped = 3,
    Delivered = 4,
    Canceled = 5,
    Refunded = 6,
}
