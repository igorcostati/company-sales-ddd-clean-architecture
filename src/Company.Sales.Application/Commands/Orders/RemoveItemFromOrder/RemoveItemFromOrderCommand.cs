using System;

namespace Company.Sales.Application.Commands.Orders.RemoveItemFromOrder;

public sealed class RemoveItemFromOrderCommand
{
    public Guid OrderId { get; }
    public Guid ItemId { get; }

    public RemoveItemFromOrderCommand(
        Guid orderId,
        Guid itemId)
    {
        OrderId = orderId;
        ItemId = itemId;
    }
}