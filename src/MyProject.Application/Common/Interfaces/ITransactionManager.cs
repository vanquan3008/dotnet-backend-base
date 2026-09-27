namespace MyProject.Application.Common.Interfaces;

public interface ITransactionManager
{
    Task<TResponse> ExecuteAsync<TResponse>(
        Func<CancellationToken, Task<TResponse>> operation,
        CancellationToken cancellationToken = default);
}
