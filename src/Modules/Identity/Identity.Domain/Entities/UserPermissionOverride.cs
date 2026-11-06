using Identity.Domain.Security;

namespace Identity.Domain.Entities;

public sealed class UserPermissionOverride
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public long SalonId { get; private set; }
    public string Permission { get; private set; } = null!;
    public UserPermissionEffect Effect { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long UpdatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private UserPermissionOverride()
    {
    }

    public static UserPermissionOverride Create(
        long userId,
        long salonId,
        string permission,
        UserPermissionEffect effect,
        long actorUserId,
        DateTimeOffset now) =>
        new()
        {
            UserId = userId,
            SalonId = salonId,
            Permission = permission,
            Effect = effect,
            CreatedBy = actorUserId,
            CreatedAt = now,
            UpdatedBy = actorUserId,
            UpdatedAt = now,
            IsActive = true
        };

    public void SetEffect(UserPermissionEffect effect, long actorUserId, DateTimeOffset now)
    {
        Effect = effect;
        UpdatedBy = actorUserId;
        UpdatedAt = now;
        IsActive = true;
    }

    public void Deactivate(long actorUserId, DateTimeOffset now)
    {
        IsActive = false;
        UpdatedBy = actorUserId;
        UpdatedAt = now;
    }
}
