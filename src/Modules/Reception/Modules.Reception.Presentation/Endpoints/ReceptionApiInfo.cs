using Microsoft.AspNetCore.Http;

namespace Modules.Reception.Presentation.Endpoints;

public static class ReceptionApiInfo
{
    public const string Tag = "Receptions";
    public const string Prefix = "/api/Receptions";

    public static bool TryGetSalonId(HttpContext context, out long salonId) =>
        long.TryParse(context.User.FindFirst("SalonId")?.Value, out salonId) && salonId > 0;
}
