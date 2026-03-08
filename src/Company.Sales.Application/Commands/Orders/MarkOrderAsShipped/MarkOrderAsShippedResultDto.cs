using System;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsShipped;

public class MarkOrderAsShippedResultDto
{
    public Guid OrderId { get; init; }
    public string OrderStatus { get; init; } = string.Empty;

}
