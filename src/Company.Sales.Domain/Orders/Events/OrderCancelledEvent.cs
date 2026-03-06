using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Orders.Enums;
using Company.Sales.Domain.Orders.ValuesObjects;

namespace Company.Sales.Domain.Orders.Events;

public sealed record OrderCancelledEvent(
    Guid OrderId,
    Guid ClientID,
    OrderStatus PreviousStatus,
    CancellationReason CancellationReason,
    Guid? PaymentId
) : DomainEventBase;
