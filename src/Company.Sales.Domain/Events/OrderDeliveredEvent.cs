using Company.Sales.Domain.Events;

namespace Company.Sales.Domain.Events;

public sealed record OrderDeliveredEvent(
    Guid OrderId,
    Guid ClientID
) : DomainEventBase;
