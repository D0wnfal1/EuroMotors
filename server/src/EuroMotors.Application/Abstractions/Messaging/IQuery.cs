using EuroMotors.SharedKernel;
using MediatR;

namespace EuroMotors.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
