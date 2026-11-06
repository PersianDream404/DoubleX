using Identity.Application.Contract.Security;
using Identity.Domain.Entities;
using Identity.Domain.Security;
using Identity.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence.Repositories.Users;

public sealed class UserPermissionOverrideRepository(
    IdentityWriteDbContext context,
    UserManager<ApplicationUser> userManager)
    : IUserPermissionOverrideRepository
{
    public async Task<UserPermissionTarget?> GetTargetUserAsync(
        long userId,
        long salonId,
        CancellationToken cancellationToken)
    {
        var target = await GetUserAsync(userId, cancellationToken);
        return target?.SalonId == salonId ? target : null;
    }

    public async Task<UserPermissionTarget?> GetUserAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new UserPermissionTarget(user.Id, user.SalonId, roles.ToArray());
    }

    public async Task<IReadOnlyCollection<UserPermissionOverride>> GetActiveOverridesAsync(
        long userId,
        long salonId,
        CancellationToken cancellationToken) =>
        await context.UserPermissionOverrides
            .AsNoTracking()
            .Where(item => item.UserId == userId &&
                           item.SalonId == salonId &&
                           item.IsActive)
            .OrderBy(item => item.Permission)
            .ToArrayAsync(cancellationToken);

    public async Task SaveOverrideAsync(
        long userId,
        long salonId,
        string permission,
        UserPermissionEffect effect,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var item = await context.UserPermissionOverrides
            .SingleOrDefaultAsync(
                item => item.UserId == userId &&
                        item.SalonId == salonId &&
                        item.Permission == permission,
                cancellationToken);

        if (item is null)
        {
            item = UserPermissionOverride.Create(
                userId, salonId, permission, effect, actorUserId, now);
            context.UserPermissionOverrides.Add(item);
        }
        else
        {
            item.SetEffect(effect, actorUserId, now);
        }

        context.UserPermissionOverrideAudits.Add(
            UserPermissionOverrideAudit.Create(
                userId,
                salonId,
                permission,
                effect,
                effect.ToString(),
                actorUserId,
                now));
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveOverrideAsync(
        long userId,
        long salonId,
        string permission,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var item = await context.UserPermissionOverrides
            .SingleOrDefaultAsync(
                item => item.UserId == userId &&
                        item.SalonId == salonId &&
                        item.Permission == permission &&
                        item.IsActive,
                cancellationToken);
        if (item is null)
        {
            return;
        }

        item.Deactivate(actorUserId, now);
        context.UserPermissionOverrideAudits.Add(
            UserPermissionOverrideAudit.Create(
                userId,
                salonId,
                permission,
                item.Effect,
                "Removed",
                actorUserId,
                now));
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetOverridesAsync(
        long userId,
        long salonId,
        long actorUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var activeOverrides = await context.UserPermissionOverrides
            .Where(item => item.UserId == userId &&
                           item.SalonId == salonId &&
                           item.IsActive)
            .ToArrayAsync(cancellationToken);
        if (activeOverrides.Length == 0)
        {
            return;
        }

        foreach (var item in activeOverrides)
        {
            item.Deactivate(actorUserId, now);
            context.UserPermissionOverrideAudits.Add(
                UserPermissionOverrideAudit.Create(
                    userId,
                    salonId,
                    item.Permission,
                    item.Effect,
                    "Reset",
                    actorUserId,
                    now));
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
