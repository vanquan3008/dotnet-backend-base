using MediatR;
using MyProject.Application.Common.Interfaces;
using MyProject.Application.Common.Messaging.Commands;

namespace MyProject.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    ITransactionManager transactionManager)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalCommandMarker)
        {
            return await next(cancellationToken);
        }
        return await transactionManager.ExecuteAsync(
            async token =>
            {
                return await next(token);
            },
            cancellationToken);
    }
}
