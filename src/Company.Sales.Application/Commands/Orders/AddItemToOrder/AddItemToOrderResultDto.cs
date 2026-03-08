using System;

namespace Company.Sales.Application.Commands.Orders.AddItemToOrder;

public sealed class AddItemToOrderResultDto
{
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string Status { get; }

    public AddItemToOrderResultDto(
        Guid orderId,
        decimal totalAmount,
        string status)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
        Status = status;
    }
}