namespace Company.Sales.Domain.Events;

public interface IDomainEvent
{
    DateTime DateOccurred { get; }
}
