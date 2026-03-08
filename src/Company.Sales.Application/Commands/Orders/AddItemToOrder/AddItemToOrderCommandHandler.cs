using System;
using Company.Sales.Application.Abstractions.Persistence;

namespace Company.Sales.Application.Commands.Orders.AddItemToOrder;

public sealed class AddItemToOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public AddItemToOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<AddItemToOrderResultDto> HandleAsync(
        AddItemToOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order not found.");

        order.AddItem(
            command.ProductId,
            command.ProductName,
            command.UnitPrice,
            command.Quantity
        );

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new AddItemToOrderResultDto(
            order.Id,
            order.TotalAmount,
            order.OrderStatus.ToString()
        );
    }
}
