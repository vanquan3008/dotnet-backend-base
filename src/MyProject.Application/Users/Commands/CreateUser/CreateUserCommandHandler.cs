using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Interfaces;
using MyProject.Application.Common.Messaging.Commands;
using MyProject.Application.Users.Models;
using MyProject.Domain.Entities;
using MyProject.Domain.Errors.Users;
using MyProject.Domain.Repositories;
using MyProject.Domain.ValueObjects.Users;

namespace MyProject.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(IUserRepository users, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var code = Code.Create(request.Code);
        var name = Name.Create(request.Name);
        var email = Email.Create(request.Email);

        if (await users.ExistsByCodeAsync(code, cancellationToken))
        {
            throw new ConflictException(UserErrorCodes.CodeAlreadyExists, "User code already exists.");
        }

        if (await users.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ConflictException(UserErrorCodes.EmailAlreadyExists, "User email already exists.");
        }

        var user = User.Create(Guid.NewGuid(), code, name, email);
        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UserDto.From(user);
    }
}
