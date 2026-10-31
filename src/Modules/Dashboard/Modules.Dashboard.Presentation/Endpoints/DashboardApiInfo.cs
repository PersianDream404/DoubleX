using Microsoft.AspNetCore.Http;

namespace Modules.Dashboard.Presentation.Endpoints;

public static class DashboardApiInfo
{
    public const string Tag = "Dashboard";
    public const string Prefix = "/api/Dashboard";

    public static bool TryGetSalonId(HttpContext context, out long salonId) =>
        long.TryParse(context.User.FindFirst("SalonId")?.Value, out salonId) && salonId > 0;
}
