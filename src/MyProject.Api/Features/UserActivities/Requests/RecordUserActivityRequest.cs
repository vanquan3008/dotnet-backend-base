using System.ComponentModel.DataAnnotations;
using MyProject.Application.UserActivities.Models;

namespace MyProject.Api.Features.UserActivities.Requests;

public sealed record RecordUserActivityRequest(
    [Required, MaxLength(UserActivity.TypeMaxLength)] string Type,
    [MaxLength(UserActivity.DetailsMaxLength)] string? Details);
