using MyProject.Application.Users.Models;

namespace MyProject.Api.Features.Users.Responses;

public sealed record UserResponse(
    Guid Id,
    string Code,
    string Name,
    string Email,
    string Status,
    DateTimeOffset CreatedAt)
{
    public static UserResponse From(UserDto user)
        => new(user.Id, user.Code, user.Name, user.Email, user.Status, user.CreatedAt);
}
