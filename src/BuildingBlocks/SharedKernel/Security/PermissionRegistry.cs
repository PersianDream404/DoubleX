using SharedKernel.Interface;

namespace SharedKernel.Security;

public sealed class PermissionRegistry
{
    private readonly HashSet<string> _permissionSet;

    public PermissionRegistry(IEnumerable<IModulePermissionProvider> providers)
    {
        var permissions = providers
            .SelectMany(provider => provider.GetPermissions())
            .ToArray();

        var invalidPermissions = permissions
            .Where(permission => !IsValid(permission))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (invalidPermissions.Length > 0)
        {
            throw new InvalidOperationException(
                $"Invalid permissions were registered: {string.Join(", ", invalidPermissions)}.");
        }

        var duplicates = permissions
            .GroupBy(permission => permission, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException(
                $"Duplicate permissions were registered: {string.Join(", ", duplicates)}.");
        }

        All = Array.AsReadOnly(permissions.OrderBy(permission => permission, StringComparer.Ordinal).ToArray());
        _permissionSet = new HashSet<string>(permissions, StringComparer.Ordinal);
    }

    public IReadOnlyCollection<string> All { get; }
    public IReadOnlyCollection<string> Assignable => All
        .Where(permission => !permission.StartsWith("Identity.", StringComparison.Ordinal))
        .ToArray();

    public bool Contains(string permission) => _permissionSet.Contains(permission);

    public bool IsAssignable(string permission) =>
        _permissionSet.Contains(permission) &&
        !permission.StartsWith("Identity.", StringComparison.Ordinal);

    private static bool IsValid(string? permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        var segments = permission.Split('.');
        return segments.Length >= 2 &&
               segments.All(segment =>
                   segment.Length > 0 &&
                   char.IsAsciiLetter(segment[0]) &&
                   segment.All(char.IsAsciiLetterOrDigit));
    }
}
