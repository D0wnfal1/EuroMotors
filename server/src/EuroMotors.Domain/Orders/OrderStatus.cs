namespace EuroMotors.Domain.Orders;

public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Shipped = 3,
    Completed = 4,
    Canceled = 5,
    Refunded = 6,
}
