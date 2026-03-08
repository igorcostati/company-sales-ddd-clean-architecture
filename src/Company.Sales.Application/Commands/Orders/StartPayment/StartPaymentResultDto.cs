using System;

namespace Company.Sales.Application.Commands.Orders.StartPayment;


public sealed class StartPaymentResultDto
{
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public string OrderStatus { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
}
