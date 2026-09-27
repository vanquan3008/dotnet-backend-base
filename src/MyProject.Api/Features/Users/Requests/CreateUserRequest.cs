using System.ComponentModel.DataAnnotations;

namespace MyProject.Api.Features.Users.Requests;

public sealed record CreateUserRequest(
    [Required, MaxLength(MyProject.Domain.ValueObjects.Users.Code.MaxLength)] string Code,
    [Required,
     MinLength(MyProject.Domain.ValueObjects.Users.Name.MinLength),
     MaxLength(MyProject.Domain.ValueObjects.Users.Name.MaxLength)] string Name,
    [Required, EmailAddress, MaxLength(MyProject.Domain.ValueObjects.Users.Email.MaxLength)] string Email);
