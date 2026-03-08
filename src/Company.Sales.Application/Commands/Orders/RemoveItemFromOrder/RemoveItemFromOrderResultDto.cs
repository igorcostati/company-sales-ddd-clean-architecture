using System;

namespace Company.Sales.Application.Commands.Orders.RemoveItemFromOrder;

public sealed class RemoveItemFromOrderResultDto
{
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string Status { get; }

    public RemoveItemFromOrderResultDto(
        Guid orderId,
        decimal totalAmount,
        string status)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
        Status = status;
    }
}
