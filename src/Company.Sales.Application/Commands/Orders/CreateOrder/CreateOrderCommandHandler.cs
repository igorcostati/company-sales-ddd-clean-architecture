using System;
using Company.Sales.Application.Abstractions.Persistence;
using Company.Sales.Domain.Orders.Entities;

namespace Company.Sales.Application.Commands.Orders.CreateOrder;


public sealed class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CreateOrderResultDto> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = Order.Create(
            command.CustomerId,
            command.DeliveryAddress
        );

        await _orderRepository.AddAsync(order, cancellationToken);

        return new CreateOrderResultDto(
            order.Id,
            order.OrderNumber,
            order.CreatedAt,
            order.TotalAmount,
            order.OrderStatus.ToString()
        );
    }
}
