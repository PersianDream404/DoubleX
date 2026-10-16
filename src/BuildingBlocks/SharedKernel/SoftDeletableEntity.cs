using SharedKernel.Base;
using SharedKernel.Interface;

namespace SmeOpsHub.SharedKernel;

public abstract class SoftDeletableEntity<TId> : Entity<TId>, ISoftDeletable
{
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    protected SoftDeletableEntity() : base()
    {
    }

    protected SoftDeletableEntity(TId id) : base(id)
    {
    }

    public void SoftDelete(string? deletedBy)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
