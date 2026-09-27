using System.Net;

namespace MyProject.Application.Common.Interfaces;

public interface IExternalApiClient
{
    Task<ExternalApiResponse<TResult>> SendAsync<TResult>(
        ExternalApiRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record ExternalApiRequest(
    string BaseUrl,
    string Endpoint,
    HttpMethod Method,
    object? Body = null,
    string? BearerToken = null,
    string? ApiKey = null,
    string ApiKeyHeader = "X-Api-Key",
    IReadOnlyDictionary<string, string>? Headers = null,
    string ContentType = "application/json",
    int TimeoutSeconds = 30,
    bool LogPayload = false);

public sealed record ExternalApiResponse<TResult>(
    bool IsSuccess,
    HttpStatusCode? StatusCode,
    TResult? Result,
    string? Content,
    string? ErrorMessage,
    long DurationMilliseconds);
