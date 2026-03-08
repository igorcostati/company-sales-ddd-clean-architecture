using System;
using Company.Sales.Application.Abstractions.Persistence;
using Company.Sales.Domain.Common.Exceptions;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsDelivered;

public sealed class MarkOrderAsDeliveredCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public MarkOrderAsDeliveredCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<MarkOrderAsDeliveredResultDto> HandleAsync(
        MarkOrderAsDeliveredCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
                        command.OrderId,
                        cancellationToken)
                    ?? throw new DomainException("Order not found.");

        order.MarkAsDelivered();

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new MarkOrderAsDeliveredResultDto
        {
            OrderId = order.Id,
            Status = order.OrderStatus.ToString()
        };
    }
}
