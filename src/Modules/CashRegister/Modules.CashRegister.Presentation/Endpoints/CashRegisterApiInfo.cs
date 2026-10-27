using Microsoft.AspNetCore.Http;

namespace Modules.CashRegister.Presentation.Endpoints;

public static class CashRegisterApiInfo
{
    public const string Tag = "CashRegisters";
    public const string Prefix = "/api/CashRegisters";

    public static bool TryGetSalonId(HttpContext context, out long salonId) =>
        long.TryParse(context.User.FindFirst("SalonId")?.Value, out salonId) && salonId > 0;

    public static bool TryGetIdentity(HttpContext context, out long salonId, out long userId)
    {
        userId = 0;
        return TryGetSalonId(context, out salonId) &&
               long.TryParse(context.User.FindFirst("UserId")?.Value, out userId) &&
               userId > 0;
    }
}
