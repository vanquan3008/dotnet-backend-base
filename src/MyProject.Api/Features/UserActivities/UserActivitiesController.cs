using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Api.Features.UserActivities.Requests;
using MyProject.Api.Features.UserActivities.Responses;
using MyProject.Application.UserActivities.Commands.RecordUserActivity;
using MyProject.Application.UserActivities.Queries.GetUserActivities;

namespace MyProject.Api.Features.UserActivities;

[ApiController]
[Route("api/users/{userId:guid}/activities")]
public sealed class UserActivitiesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<UserActivityResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserActivityResponse>> Record(
        Guid userId,
        RecordUserActivityRequest request,
        CancellationToken cancellationToken)
    {
        var activity = await sender.Send(
            new RecordUserActivityCommand(userId, request.Type, request.Details),
            cancellationToken);
        var response = UserActivityResponse.From(activity);
        return CreatedAtAction(nameof(GetRecent), new { userId }, response);
    }

    [HttpGet]
    [ProducesResponseType<UserActivitiesResponse>(StatusCodes.Status200OK)]
    public async Task<UserActivitiesResponse> GetRecent(
        Guid userId,
        [FromQuery, Range(1, 100)] int limit = 50,
        CancellationToken cancellationToken = default)
        => UserActivitiesResponse.From(
            await sender.Send(new GetUserActivitiesQuery(userId, limit), cancellationToken));
}
