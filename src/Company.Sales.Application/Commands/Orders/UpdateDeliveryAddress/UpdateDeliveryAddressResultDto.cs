using System;

namespace Company.Sales.Application.Commands.Orders.UpdateDeliveryAddress;

public sealed class UpdateDeliveryAddressResultDto
{
    public Guid OrderId { get; }
    public string DeliveryAddress { get; }
    public string Status { get; }

    public UpdateDeliveryAddressResultDto(
        Guid orderId,
        string deliveryAddress,
        string status)
    {
        OrderId = orderId;
        DeliveryAddress = deliveryAddress;
        Status = status;
    }
}
