using MyProject.Domain.Errors.Users;
using MyProject.Domain.Exceptions;

namespace MyProject.Domain.ValueObjects.Users;

public sealed record Email
{
    public const int MaxLength = 320;
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(UserErrorCodes.EmailRequired, "Email is required.");
        }

        value = value.Trim().ToLowerInvariant();
        if (value.Length > MaxLength)
        {
            throw new DomainException(UserErrorCodes.EmailTooLong, $"Email must not exceed {MaxLength} characters.");
        }

        var at = value.IndexOf('@');
        if (at <= 0 || at == value.Length - 1 || at != value.LastIndexOf('@') || value.Any(char.IsWhiteSpace))
        {
            throw new DomainException(UserErrorCodes.EmailInvalid, "Email is invalid.");
        }

        return new Email(value);
    }

    public override string ToString() => Value;
}
