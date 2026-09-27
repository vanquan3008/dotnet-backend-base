using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Api.Features.Users.Requests;
using MyProject.Api.Features.Users.Responses;
using MyProject.Application.Users.Commands.CreateUser;
using MyProject.Application.Users.Queries.GetUser;

namespace MyProject.Api.Features.Users;

[ApiController]
[Route("api/users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new CreateUserCommand(request.Code, request.Name, request.Email),
            cancellationToken);
        var response = UserResponse.From(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<UserResponse> GetById(Guid id, CancellationToken cancellationToken)
        => UserResponse.From(await sender.Send(new GetUserQuery(id), cancellationToken));
}
