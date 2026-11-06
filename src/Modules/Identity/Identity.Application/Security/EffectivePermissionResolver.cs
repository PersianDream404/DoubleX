using Identity.Application.Contract.Security;
using Identity.Application.Security;
using Identity.Domain.Security;
using SharedKernel.Security;

namespace Identity.Application.Security;

public sealed class EffectivePermissionResolver(
    IUserPermissionOverrideRepository repository,
    PermissionRegistry permissionRegistry)
    : IEffectivePermissionResolver
{
    public async Task<IReadOnlyCollection<string>> ResolveAsync(
        long userId,
        long salonId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken)
    {
        var permissions = RolePermissions.GetPermissions(roles, permissionRegistry);
        var effective = new HashSet<string>(permissions, StringComparer.Ordinal);
        var overrides = await repository.GetActiveOverridesAsync(userId, salonId, cancellationToken);

        var denied = overrides
            .Where(item => item.Effect == UserPermissionEffect.Deny)
            .Select(item => item.Permission)
            .ToHashSet(StringComparer.Ordinal);
        var granted = overrides
            .Where(item => item.Effect == UserPermissionEffect.Grant)
            .Select(item => item.Permission);

        effective.UnionWith(granted);
        effective.ExceptWith(denied);
        return effective.OrderBy(permission => permission, StringComparer.Ordinal).ToArray();
    }
}
