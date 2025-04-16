using EuroMotors.Application.Abstractions.Callback;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Callback.RequestCallback;

public class RequestCallbackCommandHandler(ICallbackService callbackService) : ICommandHandler<RequestCallbackCommand>
{
    public async Task<Result> Handle(RequestCallbackCommand request, CancellationToken cancellationToken)
    {
        await callbackService.SendMessageAsync(request.Name, request.Phone);
        return Result.Success();
    }
}
