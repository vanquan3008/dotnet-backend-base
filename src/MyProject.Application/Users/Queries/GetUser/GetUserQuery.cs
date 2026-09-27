using MyProject.Application.Common.Messaging.Queries;
using MyProject.Application.Users.Models;

namespace MyProject.Application.Users.Queries.GetUser;

public sealed record GetUserQuery(Guid Id) : IQuery<UserDto>;
