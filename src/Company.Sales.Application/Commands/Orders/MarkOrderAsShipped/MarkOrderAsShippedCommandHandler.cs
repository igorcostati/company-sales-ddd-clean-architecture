using System;
using Company.Sales.Application.Abstractions.Persistence;
using Company.Sales.Domain.Common.Exceptions;

namespace Company.Sales.Application.Commands.Orders.MarkOrderAsShipped;

public sealed class MarkOrderAsShippedCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public MarkOrderAsShippedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<MarkOrderAsShippedResultDto> HandleAsync(
        MarkOrderAsShippedCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken)
            ?? throw new DomainException("Order not found.");

        order.MarkAsShipped(); 

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new MarkOrderAsShippedResultDto
        {
            OrderId = order.Id,
            OrderStatus = order.OrderStatus.ToString()
        };
    }
}
