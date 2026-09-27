using System.ComponentModel.DataAnnotations;
using MyProject.Application.Common.Messaging.Commands;
using MyProject.Application.Users.Models;
using UserCode = MyProject.Domain.ValueObjects.Users.Code;
using UserEmail = MyProject.Domain.ValueObjects.Users.Email;
using UserName = MyProject.Domain.ValueObjects.Users.Name;

namespace MyProject.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand : ICommand<UserDto>
{
    [Required]
    [MaxLength(UserCode.MaxLength)]
    public string Code { get; init; }

    [Required]
    [MinLength(UserName.MinLength)]
    [MaxLength(UserName.MaxLength)]
    public string Name { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(UserEmail.MaxLength)]
    public string Email { get; init; }

    public CreateUserCommand(
        string code,
        string name,
        string email)
    {
        Code = code;
        Name = name;
        Email = email;
    }
}
