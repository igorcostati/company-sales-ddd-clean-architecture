using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Orders.ValuesObjects;

namespace Company.Sales.Domain.Orders.Events;

public sealed record OrderShippedEvent
(
    Guid OrderId,
    Guid ClientID,
    DeliveryAddress DeliveryAddress) : DomainEventBase;
