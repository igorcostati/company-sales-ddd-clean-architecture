namespace Company.Sales.Domain.Events;

public record PaymentRejectedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime PaymentDate,
    string? TransactionCode) : DomainEventBase;