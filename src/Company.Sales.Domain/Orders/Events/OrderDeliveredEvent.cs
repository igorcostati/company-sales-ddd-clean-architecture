using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Orders.Events;

public sealed record OrderDeliveredEvent(
    Guid OrderId,
    Guid ClientID
) : DomainEventBase;
