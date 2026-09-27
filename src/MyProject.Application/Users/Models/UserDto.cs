using MyProject.Domain.Entities;

namespace MyProject.Application.Users.Models;

public sealed record UserDto(
    Guid Id,
    string Code,
    string Name,
    string Email,
    string Status,
    DateTimeOffset CreatedAt)
{
    public static UserDto From(User user) => new(
        user.Id,
        user.Code.Value,
        user.Name.Value,
        user.Email.Value,
        user.Status.ToString(),
        user.CreatedAt);
}
