using EuroMotors.Domain.Abstractions;
using MediatR;

namespace EuroMotors.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
