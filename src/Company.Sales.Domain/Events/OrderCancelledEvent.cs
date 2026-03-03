using Company.Sales.Domain.Common.Enums;
using Company.Sales.Domain.ValuesObjects;

namespace Company.Sales.Domain.Events;

public sealed record OrderCancelledEvent(
    Guid OrderId,
    Guid ClientID,
    OrderStatus PreviousStatus,
    CancellationReason CancellationReason,
    Guid? PaymentId
) : DomainEventBase;
