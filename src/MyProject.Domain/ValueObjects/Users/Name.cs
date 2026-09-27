using MyProject.Domain.Errors.Users;
using MyProject.Domain.Exceptions;

namespace MyProject.Domain.ValueObjects.Users;

public sealed record Name
{
    public const int MinLength = 2;
    public const int MaxLength = 200;
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Name Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(UserErrorCodes.NameRequired, "Name is required.");
        }

        value = value.Trim();
        if (value.Length < MinLength)
        {
            throw new DomainException(UserErrorCodes.NameTooShort, $"Name must be at least {MinLength} characters.");
        }

        if (value.Length > MaxLength)
        {
            throw new DomainException(UserErrorCodes.NameTooLong, $"Name must not exceed {MaxLength} characters.");
        }

        return new Name(value);
    }

    public override string ToString() => Value;
}
