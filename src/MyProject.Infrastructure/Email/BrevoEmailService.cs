using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Email;

internal sealed class BrevoEmailService(HttpClient client, IOptions<EmailOptions> options)
    : IEmailService
{
    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        using var request = new HttpRequestMessage(HttpMethod.Post, "smtp/email")
        {
            Content = JsonContent.Create(new
            {
                sender = new { name = settings.SenderName, email = settings.SenderEmail },
                to = new[] { new { name = message.ToName, email = message.ToEmail } },
                subject = message.Subject,
                htmlContent = message.HtmlContent
            })
        };
        request.Headers.Add("api-key", settings.ApiKey);
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
