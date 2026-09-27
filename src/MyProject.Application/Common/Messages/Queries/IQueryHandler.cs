using MediatR;

namespace MyProject.Application.Common.Messaging.Queries;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
