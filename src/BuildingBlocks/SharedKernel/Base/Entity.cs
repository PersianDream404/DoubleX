namespace SharedKernel.Base;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    public TId Id { get; protected init; } = default!;

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = (Entity<TId>)obj;
        return Equals(other);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // If both are transient (default Id), they are not considered equal
        if (Equals(Id, default(TId)) && Equals(other.Id, default(TId)))
            return false;

        return Equals(Id, other.Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        => !Equals(left, right);

    public override int GetHashCode()
    {
        return Equals(Id, default(TId))
            ? base.GetHashCode()
            : Id!.GetHashCode();
    }

    public override string ToString()
        => $"{GetType().Name} [Id={Id}]";
}
