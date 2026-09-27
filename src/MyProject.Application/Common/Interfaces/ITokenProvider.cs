namespace MyProject.Application.Common.Interfaces;

public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

public interface ITokenProvider
{
    AccessToken Create(Guid userId, string email, IEnumerable<string> roles);
}
