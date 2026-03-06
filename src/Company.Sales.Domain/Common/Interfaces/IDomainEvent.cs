namespace Company.Sales.Domain.Common.Interfaces;

public interface IDomainEvent
{
    DateTime DateOccurred { get; }
}
