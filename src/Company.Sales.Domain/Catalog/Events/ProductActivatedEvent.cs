using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Catalog.Events;

public sealed record ProductActivatedEvent(Guid ProductId)
    : DomainEventBase;