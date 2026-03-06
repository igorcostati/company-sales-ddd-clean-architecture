using Company.Sales.Domain.Common.Base; 

namespace Company.Sales.Domain.Customers.Events;


public sealed record CustomerBlockedEvent(
    Guid CustomerId,
    string Cpf) : DomainEventBase;


