using Company.Sales.Domain.Common.Base;

namespace Company.Sales.Domain.Catalog.Events;

public sealed record StockAdjustedEvent(
    Guid ProductId,
    int Quantity,
    string Reason)
    : DomainEventBase;
