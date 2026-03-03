using Company.Sales.Domain.Events;
using Company.Sales.Domain.ValuesObjects;

namespace Company.Sales.Domain.Events;

public sealed record OrderShippedEvent
(
    Guid OrderId,
    Guid ClientID,
    DeliveryAddress DeliveryAddress) : DomainEventBase;
