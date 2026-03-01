namespace Company.Sales.Domain.Common.Base;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    protected Entity(Guid id)
    {
        Id = id;
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id.Equals(other.Id);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }    

    public static bool operator ==(Entity left, Entity right)
    {
        if(ReferenceEquals(left, null))
            return ReferenceEquals(right, null);
        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}
