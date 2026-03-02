namespace Company.Sales.Domain.Events;

public record PaymentApprovedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime PaymentDate,
    string? TransactionCode) : DomainEventBase;