using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Identity;

internal sealed class CurrentUser(IHttpContextAccessor contextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => contextAccessor.HttpContext?.User;

    public Guid? UserId
        => Guid.TryParse(
            Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Principal?.FindFirstValue("sub"),
            out var userId)
            ? userId
            : null;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles
        => Principal?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray() ?? [];
}
