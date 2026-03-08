using System;
using Company.Sales.Domain.Orders.Entities;

namespace Company.Sales.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Order order,
        CancellationToken cancellationToken = default);
}