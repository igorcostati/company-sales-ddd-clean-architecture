using System;
using Company.Sales.Domain.Orders.Enums;

namespace Company.Sales.Application.Commands.Orders.StartPayment;

public sealed class StartPaymentCommand
{
    public Guid OrderId { get; }
    public PaymentMethod PaymentMethod { get; }

    public StartPaymentCommand(Guid orderId, PaymentMethod paymentMethod)
    {
        OrderId = orderId;
        PaymentMethod = paymentMethod;
    }
}
