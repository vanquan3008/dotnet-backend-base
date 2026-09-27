using MediatR;

namespace MyProject.Application.Common.Messaging.Commands;

public interface ICommand : IRequest;
public interface ICommand<TResponse> : IRequest<TResponse>;
public interface ITransactionalCommandMarker;
public interface ITransactionalCommand : ICommand, ITransactionalCommandMarker;
public interface ITransactionalCommand<TResponse> : ICommand<TResponse>, ITransactionalCommandMarker;
