using System;
using Company.Sales.Domain.Orders.ValuesObjects;

namespace Company.Sales.Application.Commands.Orders.CreateOrder;

public sealed class CreateOrderCommand
{
    public Guid CustomerId { get; }
    public DeliveryAddress DeliveryAddress { get; }

    public CreateOrderCommand(
        Guid customerId,
        DeliveryAddress deliveryAddress)
    {
        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
    }
}