namespace EuroMotors.Application.Abstractions.Callback;

public interface ICallbackService
{
    Task SendMessageAsync(string name, string phone);
}
