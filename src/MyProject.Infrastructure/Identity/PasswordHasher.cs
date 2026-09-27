using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Identity;

internal sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> hasher = new();
    private readonly object user = new();

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return hasher.HashPassword(user, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        try
        {
            return hasher.VerifyHashedPassword(user, passwordHash, password)
                != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
