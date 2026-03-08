using System;

namespace Company.Sales.Application.Commands.Orders.CancelOrder;

public sealed class CancelOrderResultDto
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
}