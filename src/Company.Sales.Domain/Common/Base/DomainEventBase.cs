using Company.Sales.Domain.Common.Interfaces;

namespace Company.Sales.Domain.Common.Base;

public abstract record class DomainEventBase : IDomainEvent
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
