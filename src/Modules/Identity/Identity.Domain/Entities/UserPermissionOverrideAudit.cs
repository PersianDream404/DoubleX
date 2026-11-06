using Identity.Domain.Security;

namespace Identity.Domain.Entities;

public sealed class UserPermissionOverrideAudit
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public long SalonId { get; private set; }
    public string Permission { get; private set; } = null!;
    public UserPermissionEffect? Effect { get; private set; }
    public string Action { get; private set; } = null!;
    public long ActorUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private UserPermissionOverrideAudit()
    {
    }

    public static UserPermissionOverrideAudit Create(
        long userId,
        long salonId,
        string permission,
        UserPermissionEffect? effect,
        string action,
        long actorUserId,
        DateTimeOffset now) =>
        new()
        {
            UserId = userId,
            SalonId = salonId,
            Permission = permission,
            Effect = effect,
            Action = action,
            ActorUserId = actorUserId,
            CreatedAt = now
        };
}
