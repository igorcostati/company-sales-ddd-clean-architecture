using System;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsDelivered;

public sealed class MarkOrderAsDeliveredCommand
{
    public Guid OrderId { get; }

    public MarkOrderAsDeliveredCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
