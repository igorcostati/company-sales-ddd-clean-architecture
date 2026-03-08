using System;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsShipped;

public sealed class MarkOrderAsShippedCommand
{
    public Guid OrderId { get; }

    public MarkOrderAsShippedCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
