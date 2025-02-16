using System.Security.Claims;

namespace Infrastructure.Extensions;

public static class UserExtensions
{
    public static Guid GetId(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}