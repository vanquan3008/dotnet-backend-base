using MediatR;

namespace MyProject.Application.Common.Messaging.Queries;

public interface IQuery<TResponse> : IRequest<TResponse>;
