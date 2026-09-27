using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.ExternalServices;

public sealed class ExternalApiClient(HttpClient client, ILogger<ExternalApiClient> logger)
    : IExternalApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ExternalApiResponse<TResult>> SendAsync<TResult>(
        ExternalApiRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BaseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Endpoint);
        ArgumentNullException.ThrowIfNull(request.Method);
        if (!Uri.TryCreate(request.BaseUrl, UriKind.Absolute, out var baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("BaseUrl must be an absolute HTTP or HTTPS URL.", nameof(request));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.TimeoutSeconds);

        var requestUri = new Uri(baseUri, request.Endpoint);
        using var message = new HttpRequestMessage(request.Method, requestUri);
        if (!string.IsNullOrWhiteSpace(request.BearerToken))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.BearerToken);
        }

        if (!string.IsNullOrWhiteSpace(request.ApiKey))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.ApiKeyHeader);
            message.Headers.TryAddWithoutValidation(request.ApiKeyHeader, request.ApiKey);
        }

        if (request.Headers is not null)
        {
            foreach (var (name, value) in request.Headers)
            {
                message.Headers.TryAddWithoutValidation(name, value);
            }
        }

        var requestContent = request.Body is null
            ? null
            : Serialize(request.Body, request.ContentType);
        if (requestContent is not null)
        {
            message.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
        }

        var stopwatch = Stopwatch.StartNew();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(request.TimeoutSeconds));
        ExternalApiResponse<TResult> result;
        try
        {
            using var response = await client.SendAsync(
                message,
                HttpCompletionOption.ResponseHeadersRead,
                timeout.Token);
            var content = await response.Content.ReadAsStringAsync(timeout.Token);
            TResult? responseValue = default;
            string? errorMessage = response.IsSuccessStatusCode
                ? null
                : $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}".TrimEnd();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    responseValue = Deserialize<TResult>(
                        content,
                        response.Content.Headers.ContentType?.MediaType);
                }
                catch (Exception exception) when (exception is JsonException or InvalidOperationException)
                {
                    errorMessage = exception.Message;
                }
            }

            result = new ExternalApiResponse<TResult>(
                response.IsSuccessStatusCode && errorMessage is null,
                response.StatusCode,
                responseValue,
                content,
                errorMessage,
                stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            result = new ExternalApiResponse<TResult>(
                false,
                null,
                default,
                null,
                $"External API request timed out after {request.TimeoutSeconds} seconds.",
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            result = new ExternalApiResponse<TResult>(
                false,
                null,
                default,
                null,
                exception.Message,
                stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            stopwatch.Stop();
        }

        Log(request, requestUri.GetLeftPart(UriPartial.Path), requestContent, result);
        return result;
    }

    private static string Serialize(object body, string contentType)
    {
        if (contentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            var serializer = new XmlSerializer(body.GetType());
            using var writer = new StringWriter();
            serializer.Serialize(writer, body);
            return writer.ToString();
        }

        if (contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            return JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
        }

        throw new NotSupportedException($"Content type '{contentType}' is not supported.");
    }

    private static TResult? Deserialize<TResult>(string content, string? contentType)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        if (typeof(TResult) == typeof(string))
        {
            return (TResult)(object)content;
        }

        if (contentType?.Contains("xml", StringComparison.OrdinalIgnoreCase) == true)
        {
            var serializer = new XmlSerializer(typeof(TResult));
            using var reader = new StringReader(content);
            return (TResult?)serializer.Deserialize(reader);
        }

        return JsonSerializer.Deserialize<TResult>(content, JsonOptions);
    }

    private void Log<TResult>(
        ExternalApiRequest request,
        string requestUrl,
        string? requestContent,
        ExternalApiResponse<TResult> response)
    {
        var values = new object?[]
        {
            request.Method,
            requestUrl,
            response.StatusCode,
            response.DurationMilliseconds,
            request.LogPayload ? requestContent : null,
            request.LogPayload ? response.Content : null,
            response.ErrorMessage
        };

        const string template = "External API {Method} {Url} returned {StatusCode} in {DurationMs} ms. "
            + "Request: {RequestPayload}; Response: {ResponsePayload}; Error: {ErrorMessage}";
        if (response.IsSuccess)
        {
            logger.LogInformation(template, values);
        }
        else
        {
            logger.LogWarning(template, values);
        }
    }
}
