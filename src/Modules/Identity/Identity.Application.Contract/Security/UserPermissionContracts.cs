using Ardalis.Result;
using Identity.Domain.Entities;
using Identity.Domain.Security;

namespace Identity.Application.Contract.Security;

public sealed record UserPermissionTarget(
    long UserId,
    long SalonId,
    IReadOnlyList<string> Roles);

public sealed record ManagedPermissionDto(
    string Code,
    string Module,
    string DisplayName,
    bool IsAssignable);

public sealed record UserPermissionAccessDto(
    long UserId,
    long SalonId,
    IReadOnlyList<string> Roles,
    IReadOnlyCollection<string> RolePermissions,
    IReadOnlyCollection<string> Grants,
    IReadOnlyCollection<string> Denies,
    IReadOnlyCollection<string> EffectivePermissions);

public interface IUserPermissionOverrideRepository
{
    Task<UserPermissionTarget?> GetTargetUserAsync(
        long userId,
        long salonId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserPermissionOverride>> GetActiveOverridesAsync(
        long userId,
        long salonId,
        CancellationToken cancellationToken);

    Task SaveOverrideAsync(
        long userId,
        long salonId,
        string permission,
        UserPermissionEffect effect,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    Task RemoveOverrideAsync(
        long userId,
        long salonId,
        string permission,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    Task ResetOverridesAsync(
        long userId,
        long salonId,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken);
}

public interface IEffectivePermissionResolver
{
    Task<IReadOnlyCollection<string>> ResolveAsync(
        long userId,
        long salonId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken);
}

public interface IUserPermissionManagementService
{
    IReadOnlyCollection<ManagedPermissionDto> GetAvailablePermissions();
    IReadOnlyCollection<string> GetAssignableRoles();

    Task<Result<UserPermissionAccessDto>> GetUserPermissionsAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        CancellationToken cancellationToken);

    Task<Result> SetOverrideAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        string permission,
        UserPermissionEffect effect,
        CancellationToken cancellationToken);

    Task<Result> RemoveOverrideAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        string permission,
        CancellationToken cancellationToken);

    Task<Result> ResetOverridesAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        CancellationToken cancellationToken);
}
