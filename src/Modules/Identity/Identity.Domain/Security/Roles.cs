namespace Identity.Domain.Security;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string SalonAdmin = "SalonAdmin";
    public const string Receptionist = "Receptionist";
    public const string Personnel = "Personnel";

    public static IReadOnlyCollection<string> All { get; } =
    [
        SuperAdmin,
        SalonAdmin,
        Receptionist,
        Personnel
    ];

    public static bool IsKnown(string role) =>
        All.Contains(role, StringComparer.Ordinal);
}
