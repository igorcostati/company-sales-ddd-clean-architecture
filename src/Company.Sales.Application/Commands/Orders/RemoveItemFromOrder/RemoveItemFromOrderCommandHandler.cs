using System;
using Company.Sales.Application.Abstractions.Persistence;

namespace Company.Sales.Application.Commands.Orders.RemoveItemFromOrder;

public sealed class RemoveItemFromOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public RemoveItemFromOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<RemoveItemFromOrderResultDto> HandleAsync(
        RemoveItemFromOrderCommand command,
        CancellationToken cancellationToken = default)
    { 
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order not found.");
 
        order.RemoveItem(command.ItemId);
 
        await _orderRepository.UpdateAsync(order, cancellationToken);
 
        return new RemoveItemFromOrderResultDto(
            order.Id,
            order.TotalAmount,
            order.OrderStatus.ToString()
        );
    }
}
