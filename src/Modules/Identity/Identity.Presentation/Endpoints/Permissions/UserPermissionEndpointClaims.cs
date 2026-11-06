using System.Security.Claims;
using Identity.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Identity.Presentation.Endpoints.Permissions;

internal static class UserPermissionEndpointClaims
{
    public static bool TryGetCaller(HttpContext context, out long userId, out long salonId)
    {
        userId = 0;
        salonId = 0;
        return long.TryParse(context.User.FindFirstValue(CustomClaims.User), out userId) &&
               userId > 0 &&
               long.TryParse(context.User.FindFirstValue(CustomClaims.SalonId), out salonId) &&
               salonId > 0;
    }
}
