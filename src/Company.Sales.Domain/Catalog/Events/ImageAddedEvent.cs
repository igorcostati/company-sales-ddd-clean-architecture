using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Catalog.Events;

public sealed record ImageAddedEvent(
    Guid ProductId,
    string Url,
    int Order)
    : DomainEventBase;