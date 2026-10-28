using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Modules.Finance.Presentation.Endpoints;

public static class FinanceApiInfo
{
    public const string Tag = "Finance";
    public const string Prefix = "/api/Finance";

    public static bool TryGetIdentity(HttpContext context, out long salonId, out long userId)
    {
        userId = 0;
        return long.TryParse(context.User.FindFirstValue("SalonId"), out salonId) &&
               salonId > 0 &&
               long.TryParse(context.User.FindFirstValue("UserId"), out userId) &&
               userId > 0;
    }

    public static bool TryGetSalonId(HttpContext context, out long salonId) =>
        long.TryParse(context.User.FindFirstValue("SalonId"), out salonId) && salonId > 0;
}
