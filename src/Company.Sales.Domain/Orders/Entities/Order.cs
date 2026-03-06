using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;
using Company.Sales.Domain.Orders.Enums;
using Company.Sales.Domain.Orders.Events;
using Company.Sales.Domain.Orders.ValuesObjects;

namespace Company.Sales.Domain.Orders.Entities;

public sealed class Order : AggregateRoot
{
    // ==== Domain Properties ====

    public Guid CustomerId { get; private set; }

    public DeliveryAddress DeliveryAddress { get; private set; }

    public decimal TotalAmount { get; private set; }

    public OrderStatus OrderStatus { get; private set; }

    public string OrderNumber { get; private set; } = string.Empty;

    // ==== Items ====

    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // ==== Payments ====

    private readonly List<Payment> _payments = new();

    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Order(Guid customerId, DeliveryAddress deliveryAddress)
    {
        Guard.AgainstEmptyGuid(customerId, nameof(customerId), "Invalid CustomerId.");
        Guard.AgainstNull(deliveryAddress, nameof(deliveryAddress), "Delivery address is required.");

        CustomerId = customerId;
        DeliveryAddress = deliveryAddress;
        OrderStatus = OrderStatus.Pending;
        TotalAmount = 0m;

        GenerateOrderNumber();
    }
    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Items can only be added while the order is pending."
        );

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is not null)
        {
            existing.AddUnits(quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        }

        RecalculateTotalAmount();
        SetUpdatedAt();
    }
    public void RemoveItem(Guid itemId)
    {
        Guard.AgainstEmptyGuid(itemId, nameof(itemId), "Invalid ItemId.");

        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Items can only be removed while the order is pending."
        );

        var item = _items.FirstOrDefault(i => i.Id == itemId);

        Guard.AgainstNull(item, nameof(item), "Item not found in order.");

        _items.Remove(item!); // safe due to guard above

        Guard.Against<DomainException>(
            _items.Count == 0,
            "The order must contain at least one item."
        );

        RecalculateTotalAmount();
        SetUpdatedAt();
    }
    public void UpdateDeliveryAddress(DeliveryAddress newAddress)
    {
        Guard.AgainstNull(newAddress, nameof(newAddress));

        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Delivery address can only be changed while the order is pending."
        );

        DeliveryAddress = newAddress;
        SetUpdatedAt();
    }
    public Payment StartPayment(PaymentMethod paymentMethod)
    {
        Guard.Against<DomainException>(
            !_items.Any(),
            "Cannot start payment for an order without items."
        );

        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Payment can only be started when order status is Pending."
        );

        if (_payments.Any(p => p.PaymentStatus == PaymentStatus.Pending))
            throw new DomainException("There is already a pending payment for this order.");

        var newPayment = new Payment(Id, paymentMethod, TotalAmount);

        _payments.Add(newPayment);
        SetUpdatedAt();

        // The payment itself will emit PaymentApprovedEvent or PaymentRejectedEvent.
        return newPayment;
    }
    public void HandleApprovedPayment(Guid paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);

        if (payment is null)
            return;

        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Order is not in the expected status for payment confirmation."
        );

        OrderStatus = OrderStatus.PaymentConfirmed;
        SetUpdatedAt();
    }
    public void HandleRejectedPayment(Guid paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);

        if (payment is null)
            return;

        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Pending,
            "Order is not in the expected status for payment rejection."
        );

        OrderStatus = OrderStatus.Cancelled;
        SetUpdatedAt();

        AddDomainEvent(new OrderCancelledEvent(
            Id,
            CustomerId,
            OrderStatus,
            CancellationReason.PaymentError(),
            payment.Id
        ));
    }
    public void MarkAsPreparing()
    {
        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.PaymentConfirmed,
            "Order can only move to 'Preparing' after payment is confirmed."
        );

        OrderStatus = OrderStatus.Preparing;
        SetUpdatedAt();
    }
    public void MarkAsShipped()
    {
        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Preparing,
            "Order can only be marked as 'Shipped' after being in 'Preparing' status."
        );

        OrderStatus = OrderStatus.Shipped;
        SetUpdatedAt();

        AddDomainEvent(new OrderShippedEvent(
            Id,
            CustomerId,
            DeliveryAddress
        ));
    }
    public void MarkAsDelivered()
    {
        Guard.Against<DomainException>(
            OrderStatus != OrderStatus.Shipped,
            "Order can only be marked as 'Delivered' after being 'Shipped'."
        );

        OrderStatus = OrderStatus.Delivered;
        SetUpdatedAt();

        AddDomainEvent(new OrderDeliveredEvent(
            Id,
            CustomerId
        ));
    }
    public void CancelOrder(CancellationReason? reason = null)
    {
        Guard.Against<DomainException>(
            OrderStatus >= OrderStatus.Preparing,
            "It is not possible to cancel an order that is already in 'Preparing' status or beyond."
        );

        OrderStatus = OrderStatus.Cancelled;
        SetUpdatedAt();

        AddDomainEvent(new OrderCancelledEvent(
            Id,
            CustomerId,
            OrderStatus,
            reason ?? CancellationReason.Other(),
            _payments.LastOrDefault()?.Id
        ));
    }

    public static Order Create(Guid customerId, DeliveryAddress deliveryAddress)
    => new(customerId, deliveryAddress);
    private void RecalculateTotalAmount()
    => TotalAmount = _items.Sum(i => i.TotalAmount);
    private void GenerateOrderNumber()
    => OrderNumber = $"ORD-{Id.ToString()[..8].ToUpper()}";
}
