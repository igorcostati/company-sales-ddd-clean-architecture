using Company.Sales.Domain.Common.Base; 

namespace Company.Sales.Domain.Customers.Events;

public sealed record CustomerRegisteredEvent(
    Guid CustomerId,
    string Name,
    string Cpf,
    string Email) : DomainEventBase;
