using Ardalis.Result;
using Identity.Application.Contract.Security;
using Identity.Domain.Security;
using SharedKernel.Security;

namespace Identity.Application.Security;

public sealed class UserPermissionManagementService(
    IUserPermissionOverrideRepository repository,
    PermissionRegistry permissionRegistry)
    : IUserPermissionManagementService
{
    public IReadOnlyCollection<ManagedPermissionDto> GetAvailablePermissions() =>
        permissionRegistry.All
            .Select(permission =>
            {
                var separator = permission.IndexOf('.');
                return new ManagedPermissionDto(
                    permission,
                    permission[..separator],
                    permission,
                    permissionRegistry.IsAssignable(permission));
            })
            .ToArray();

    public IReadOnlyCollection<string> GetAssignableRoles() =>
        [Roles.Receptionist, Roles.Personnel];

    public async Task<Result<UserPermissionAccessDto>> GetUserPermissionsAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        CancellationToken cancellationToken)
    {
        var access = await ValidateAccessAsync(
            actorUserId,
            targetUserId,
            salonId,
            requireDifferentUser: true,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return Result<UserPermissionAccessDto>.NotFound("کاربر موردنظر در سالن پیدا نشد.");
        }

        var target = access.Value;
        var rolePermissions = RolePermissions.GetPermissions(target.Roles, permissionRegistry);
        var overrides = await repository.GetActiveOverridesAsync(
            targetUserId, target.SalonId, cancellationToken);
        var grants = overrides
            .Where(item => item.Effect == UserPermissionEffect.Grant)
            .Select(item => item.Permission)
            .ToHashSet(StringComparer.Ordinal);
        var denies = overrides
            .Where(item => item.Effect == UserPermissionEffect.Deny)
            .Select(item => item.Permission)
            .ToHashSet(StringComparer.Ordinal);
        var effective = new HashSet<string>(rolePermissions, StringComparer.Ordinal);
        effective.UnionWith(grants);
        effective.ExceptWith(denies);

        return Result.Success(new UserPermissionAccessDto(
            target.UserId,
            target.SalonId,
            target.Roles,
            rolePermissions,
            grants.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            denies.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            effective.OrderBy(item => item, StringComparer.Ordinal).ToArray()));
    }

    public async Task<Result> SetOverrideAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        string permission,
        UserPermissionEffect effect,
        CancellationToken cancellationToken)
    {
        if (!permissionRegistry.IsAssignable(permission))
        {
            return Result.Invalid(new ValidationError(
                nameof(permission),
                "مجوز انتخاب‌شده وجود ندارد یا قابل تخصیص نیست."));
        }

        var access = await ValidateAccessAsync(
            actorUserId,
            targetUserId,
            salonId,
            requireDifferentUser: true,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return Result.NotFound("کاربر موردنظر در سالن پیدا نشد.");
        }

        await repository.SaveOverrideAsync(
            targetUserId,
            access.Value.SalonId,
            permission,
            effect,
            actorUserId,
            DateTimeOffset.UtcNow,
            cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveOverrideAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        string permission,
        CancellationToken cancellationToken)
    {
        if (!permissionRegistry.IsAssignable(permission))
        {
            return Result.Invalid(new ValidationError(
                nameof(permission),
                "مجوز انتخاب‌شده وجود ندارد یا قابل تخصیص نیست."));
        }

        var access = await ValidateAccessAsync(
            actorUserId,
            targetUserId,
            salonId,
            requireDifferentUser: true,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return Result.NotFound("کاربر موردنظر در سالن پیدا نشد.");
        }

        await repository.RemoveOverrideAsync(
            targetUserId,
            access.Value.SalonId,
            permission,
            actorUserId,
            DateTimeOffset.UtcNow,
            cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResetOverridesAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        CancellationToken cancellationToken)
    {
        var access = await ValidateAccessAsync(
            actorUserId,
            targetUserId,
            salonId,
            requireDifferentUser: true,
            cancellationToken);
        if (!access.IsSuccess)
        {
            return Result.NotFound("کاربر موردنظر در سالن پیدا نشد.");
        }

        await repository.ResetOverridesAsync(
            targetUserId,
            access.Value.SalonId,
            actorUserId,
            DateTimeOffset.UtcNow,
            cancellationToken);
        return Result.Success();
    }

    private async Task<Result<UserPermissionTarget>> ValidateAccessAsync(
        long actorUserId,
        long targetUserId,
        long salonId,
        bool requireDifferentUser,
        CancellationToken cancellationToken)
    {
        if (actorUserId <= 0 ||
            targetUserId <= 0 ||
            salonId < 0 ||
            (requireDifferentUser && actorUserId == targetUserId))
        {
            return Result<UserPermissionTarget>.NotFound();
        }

        var actor = salonId == 0
            ? await repository.GetUserAsync(actorUserId, cancellationToken)
            : await repository.GetTargetUserAsync(actorUserId, salonId, cancellationToken);
        if (actor is null)
        {
            return Result<UserPermissionTarget>.NotFound();
        }

        var isSuperAdmin = actor.Roles.Contains(Roles.SuperAdmin, StringComparer.Ordinal);
        var isSalonAdmin = actor.Roles.Contains(Roles.SalonAdmin, StringComparer.Ordinal);
        if (!isSuperAdmin && !isSalonAdmin)
        {
            return Result<UserPermissionTarget>.NotFound();
        }
        if (salonId == 0 && !isSuperAdmin)
        {
            return Result<UserPermissionTarget>.NotFound();
        }

        var target = isSuperAdmin
            ? await repository.GetUserAsync(targetUserId, cancellationToken)
            : await repository.GetTargetUserAsync(targetUserId, salonId, cancellationToken);
        if (target is null ||
            target.Roles.Contains(Roles.SuperAdmin, StringComparer.Ordinal) ||
            (!isSuperAdmin && target.Roles.Contains(Roles.SalonAdmin, StringComparer.Ordinal)))
        {
            return Result<UserPermissionTarget>.NotFound();
        }

        return Result.Success(target);
    }
}
