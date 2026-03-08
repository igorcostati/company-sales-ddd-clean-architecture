using System;
using Company.Sales.Application.Abstractions.Persistence;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Orders.ValuesObjects;

namespace Company.Sales.Application.Commands.Orders.CancelOrder;

public sealed class CancelOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CancelOrderResultDto> HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
                        command.OrderId,
                        cancellationToken)
                    ?? throw new DomainException("Order not found.");

        var reason = new CancellationReason(command.ReasonCode);

        order.CancelOrder(reason);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new CancelOrderResultDto
        {
            OrderId = order.Id,
            Status = order.OrderStatus.ToString()
        };
    }
}
