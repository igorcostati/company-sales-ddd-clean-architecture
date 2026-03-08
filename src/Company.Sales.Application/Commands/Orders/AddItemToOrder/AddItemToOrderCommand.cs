using System;

namespace Company.Sales.Application.Commands.Orders.AddItemToOrder;
public sealed class AddItemToOrderCommand
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public AddItemToOrderCommand(
        Guid orderId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}