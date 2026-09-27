namespace MyProject.Application.Common.Interfaces;

public sealed record EmailMessage(
    string ToEmail,
    string ToName,
    string Subject,
    string HtmlContent);

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
