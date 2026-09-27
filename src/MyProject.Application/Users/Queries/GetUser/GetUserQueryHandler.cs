using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Messaging.Queries;
using MyProject.Application.Users.Interfaces;
using MyProject.Application.Users.Models;

namespace MyProject.Application.Users.Queries.GetUser;

public sealed class GetUserQueryHandler(IUserQueries users) : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        => await users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("user.not_found", "User was not found.");
}
