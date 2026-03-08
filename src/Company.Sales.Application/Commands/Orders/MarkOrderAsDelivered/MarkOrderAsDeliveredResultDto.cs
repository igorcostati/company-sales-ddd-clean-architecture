using System;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsDelivered;

public sealed class MarkOrderAsDeliveredResultDto
{
    public Guid OrderId { get; init; }
    public string Status { get; init; } = string.Empty;
}