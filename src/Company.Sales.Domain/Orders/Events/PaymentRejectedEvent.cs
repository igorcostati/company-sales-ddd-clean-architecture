using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Orders.Events;

public record PaymentRejectedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime PaymentDate,
    string? TransactionCode) : DomainEventBase;