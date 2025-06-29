namespace EuroMotors.Application.Abstractions.Callback;

public interface ICallbackService
{
    Task SendMessageAsync(string name, string phone);
    
    Task SendOrderNotificationAsync(Guid orderId, string buyerName, string buyerPhone, string? buyerEmail, decimal orderTotal);
}
