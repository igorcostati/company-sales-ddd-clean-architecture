using System;

namespace Company.Sales.Application.Commands.Orders.CreateOrder;


public sealed class CreateOrderResultDto
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public DateTime CreatedAt { get; }
    public decimal TotalAmount { get; }
    public string Status { get; }

    public CreateOrderResultDto(
        Guid orderId,
        string orderNumber,
        DateTime createdAt,
        decimal totalAmount,
        string status)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        CreatedAt = createdAt;
        TotalAmount = totalAmount;
        Status = status;
    }
}
