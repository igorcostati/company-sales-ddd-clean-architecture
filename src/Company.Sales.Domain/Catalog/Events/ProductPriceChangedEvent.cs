using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Catalog.Events;


public sealed record ProductPriceChangedEvent(
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice)
    : DomainEventBase;