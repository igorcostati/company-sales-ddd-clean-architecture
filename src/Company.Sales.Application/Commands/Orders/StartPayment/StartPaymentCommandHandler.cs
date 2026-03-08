using System;
using Company.Sales.Application.Abstractions.Persistence;
using Company.Sales.Domain.Common.Exceptions;

namespace Company.Sales.Application.Commands.Orders.StartPayment;

public sealed class StartPaymentCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public StartPaymentCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<StartPaymentResultDto> HandleAsync(
        StartPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
                        command.OrderId,
                        cancellationToken)
                    ?? throw new DomainException("Order not found.");

        var payment = order.StartPayment(command.PaymentMethod);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new StartPaymentResultDto
        {
            OrderId = order.Id,
            PaymentId = payment.Id,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = payment.PaymentStatus.ToString()
        };
    }
}
