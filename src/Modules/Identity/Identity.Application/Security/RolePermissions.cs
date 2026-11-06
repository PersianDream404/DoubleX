using Identity.Domain.Security;
using Modules.Appointment.Application.Contract.Security;
using Modules.Personnel.Application.Contract.Security;
using Modules.Reception.Application.Contract.Security;
using SharedKernel.Security;

namespace Identity.Application.Security;

public static class RolePermissions
{
    private static readonly IReadOnlyDictionary<string, string[]> ExplicitMappings =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [Roles.Receptionist] =
            [
                ReceptionPermissions.Create,
                ReceptionPermissions.List,
                ReceptionPermissions.Details,
                AppointmentPermissions.Create,
                AppointmentPermissions.List,
                AppointmentPermissions.Details
            ],
            [Roles.Personnel] =
            [
                AppointmentPermissions.List,
                AppointmentPermissions.Details
            ]
        };

    public static IReadOnlyCollection<string> GetPermissions(
        IEnumerable<string> roles,
        PermissionRegistry registry)
    {
        Validate(registry);

        var permissions = new HashSet<string>(StringComparer.Ordinal);
        foreach (var role in roles.Distinct(StringComparer.Ordinal))
        {
            if (!Roles.IsKnown(role))
            {
                throw new InvalidOperationException($"Unknown role '{role}' cannot be used to issue a token.");
            }

            if (role is Roles.SuperAdmin or Roles.SalonAdmin)
            {
                permissions.UnionWith(registry.All);
            }
            else
            {
                permissions.UnionWith(ExplicitMappings[role]);
            }
        }

        return permissions.OrderBy(permission => permission, StringComparer.Ordinal).ToArray();
    }

    public static void Validate(PermissionRegistry registry)
    {
        var unknownPermissions = ExplicitMappings
            .SelectMany(mapping => mapping.Value)
            .Where(permission => !registry.Contains(permission))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (unknownPermissions.Length > 0)
        {
            throw new InvalidOperationException(
                $"Role mappings reference unregistered permissions: {string.Join(", ", unknownPermissions)}.");
        }
    }
}
