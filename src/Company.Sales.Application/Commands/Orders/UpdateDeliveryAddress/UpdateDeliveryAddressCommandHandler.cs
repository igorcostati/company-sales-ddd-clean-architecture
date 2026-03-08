
using Company.Sales.Application.Abstractions.Persistence;

namespace Company.Sales.Application.Commands.Orders.UpdateDeliveryAddress;

public sealed class UpdateDeliveryAddressCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public UpdateDeliveryAddressCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<UpdateDeliveryAddressResultDto> HandleAsync(
        UpdateDeliveryAddressCommand command,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository
            .GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order not found.");

        order.UpdateDeliveryAddress(command.NewDeliveryAddress);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new UpdateDeliveryAddressResultDto(
            order.Id,
            order.DeliveryAddress.ToString()!,
            order.OrderStatus.ToString()
        );
    }
}
