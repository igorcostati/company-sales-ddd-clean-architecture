using System.Reflection;
using Company.Sales.Domain.Common.Enums;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Orders.Events;
using Company.Sales.Domain.Orders.Entities;
using Company.Sales.Domain.Orders.ValuesObjects;
using FluentAssertions;

namespace Company.Sales.Domain.Test.Orders.Entities;

public class OrderTest
{
    private static DeliveryAddress CreateValidAddress()
    => DeliveryAddress.Create(
        "04152-001",
        "Av. Brigadeiro Faria Lima",
        "15o andar",
        "Itaim Bibi",
        "São Paulo",
        "SP",
        "Brazil");

    private static readonly Guid ValidCustomerId = Guid.NewGuid();
    private static readonly Guid ValidProductId = Guid.NewGuid();

    private static void SetOrderStatus(Order order, OrderStatus status)
    {
        typeof(Order)
            .GetProperty(nameof(Order.OrderStatus), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(order, status);
    }

    // Order creation

    [Fact(DisplayName = "Should create valid order with Pending status")]
    public void Should_Create_Valid_Order()
    {
        // Act
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(ValidCustomerId);
        order.DeliveryAddress.Should().NotBeNull();
        order.OrderStatus.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(0m);
        order.Items.Should().BeEmpty();
        order.Payments.Should().BeEmpty();
        order.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Should not create order with invalid CustomerId")]
    public void Should_Not_Create_Order_With_Invalid_CustomerId()
    {
        // Act
        Action act = () => Order.Create(Guid.Empty, CreateValidAddress());

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Invalid CustomerId.");
    }

    [Fact(DisplayName = "Should not create order without delivery address")]
    public void Should_Not_Create_Order_Without_Address()
    {
        // Act
        Action act = () => Order.Create(ValidCustomerId, null!);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Delivery address is required.");
    }

    // Items

    [Fact(DisplayName = "Should add item to order and recalculate total amount")]
    public void Should_Add_Item_To_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Act
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        // Assert
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
        order.Items.First().TotalAmount.Should().Be(200m);
    }

    [Fact(DisplayName = "Should sum quantity of existing item when adding same product")]
    public void Should_Sum_Quantity_Of_Existing_Item()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var productId = ValidProductId;

        // Act
        order.AddItem(productId, "Keyboard", 200m, 1);
        order.AddItem(productId, "Keyboard", 200m, 2);

        // Assert
        order.Items.Should().HaveCount(1);

        var item = order.Items.First();
        item.Quantity.Should().Be(3);
        item.TotalAmount.Should().Be(600m);

        order.TotalAmount.Should().Be(600m);
    }

    [Theory(DisplayName = "Should not allow adding items when order is not Pending")]
    [InlineData(OrderStatus.PaymentConfirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void Should_Not_Add_Item_When_Order_Is_Not_Pending(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.AddItem(Guid.NewGuid(), "Other", 100m, 1);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Items can only be added while the order is pending.");
    }

    [Fact(DisplayName = "Should not allow removing the last item from order")]
    public void Should_Not_Remove_Last_Item_From_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        var itemId = order.Items.First().Id;

        // Act
        Action act = () => order.RemoveItem(itemId);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("The order must contain at least one item.");
    }

    [Fact(DisplayName = "Should remove item and recalculate total when more than one item exists")]
    public void Should_Remove_Item_When_More_Than_One_Exists()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        var product1 = Guid.NewGuid();
        var product2 = Guid.NewGuid();

        order.AddItem(product1, "Mouse", 100m, 1);
        order.AddItem(product2, "Keyboard", 200m, 1);

        var itemId = order.Items.First(i => i.ProductId == product1).Id;

        // Act
        order.RemoveItem(itemId);

        // Assert
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
    }

    [Fact(DisplayName = "Should throw when removing non-existent item")]
    public void Should_Throw_When_Removing_Nonexistent_Item()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Mouse", 100m, 2);

        // Act
        Action act = () => order.RemoveItem(Guid.NewGuid());

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Item not found in order.");
    }

    [Theory(DisplayName = "Should not allow removing items when order is not Pending")]
    [InlineData(OrderStatus.PaymentConfirmed)]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void Should_Not_Remove_Item_When_Order_Is_Not_Pending(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 10m, 1);

        var itemId = order.Items.First().Id;

        SetOrderStatus(order, status);

        // Act
        Action act = () => order.RemoveItem(itemId);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Items can only be removed while the order is pending.");
    }

    //address
    [Fact(DisplayName = "Should update delivery address when Pending")]
    public void Should_Update_Address_When_Pending()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        //var newAddress = new DeliveryAddress("00000-000", "New Street", "House", "New District", "São Paulo", "SP", "Brazil");
        var newAddress = DeliveryAddress.Create("00000-000", "New Street", "House", "New District", "São Paulo", "SP", "Brazil");

        // Act
        order.UpdateDeliveryAddress(newAddress);

        // Assert
        order.DeliveryAddress.Should().Be(newAddress);
    }

    //payments
    [Fact(DisplayName = "Should start payment and keep status Pending")]
    public void Should_Start_Payment()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 2);

        // Act
        var payment = order.StartPayment(PaymentMethod.CreditCard);

        // Assert
        payment.Should().NotBeNull();
        payment.Amount.Should().Be(200m);
        order.Payments.Should().Contain(payment);
        order.OrderStatus.Should().Be(OrderStatus.Pending);
    }

    [Fact(DisplayName = "Should not start payment without items in the order")]
    public void Should_Not_Start_Payment_Without_Items()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());

        // Act
        Action act = () => order.StartPayment(PaymentMethod.Pix);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot start payment for an order without items.");
    }

    [Fact(DisplayName = "Should not start payment if there is already a pending payment")]
    public void Should_Not_Start_Payment_If_There_Is_Already_A_Pending_One()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 1);

        // Simulate the creation of a pending payment
        order.StartPayment(PaymentMethod.Pix);

        // Act
        Action act = () => order.StartPayment(PaymentMethod.CreditCard);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("There is already a pending payment for this order.");
    }

    [Fact(DisplayName = "Should change status to PaymentConfirmed when handling approved payment")]
    public void Should_Change_Status_When_HandleApprovedPayment()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);

        // Act
        order.HandleApprovedPayment(payment.Id);

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.PaymentConfirmed);
    }

    [Fact(DisplayName = "Should cancel order and raise event when handling rejected payment")]
    public void Should_Cancel_Order_When_HandleRejectedPayment()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);

        // Act
        order.HandleRejectedPayment(payment.Id);

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCancelledEvent);
    }

    [Fact(DisplayName = "Should not handle approved payment if status is not Pending")]
    public void Should_Not_HandleApprovedPayment_When_Status_Is_Not_Pending()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);

        SetOrderStatus(order, OrderStatus.Preparing); // Simulate incorrect status

        // Act
        Action act = () => order.HandleApprovedPayment(payment.Id);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Order is not in the expected status for payment confirmation.");
    }

    // state transition
    [Fact(DisplayName = "Should allow marking order as Preparing after PaymentConfirmed")]
    public void Should_Mark_As_Preparing()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 100m, 1);
        var payment = order.StartPayment(PaymentMethod.CreditCard);
        order.HandleApprovedPayment(payment.Id); // Status: PaymentConfirmed

        // Act
        order.MarkAsPreparing();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Preparing);
    }

    [Fact(DisplayName = "Should not mark as InPreparation if payment is not confirmed")]
    public void Should_Not_Mark_As_InPreparation_When_Not_Confirmed()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        // Status: Pending

        // Act
        Action act = () => order.MarkAsPreparing();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Order can only move to 'Preparing' after payment is confirmed.");
    }

    [Fact(DisplayName = "Should mark order as Shipped")]
    public void Should_Mark_As_Shipped()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Preparing);

        // Act
        order.MarkAsShipped();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Shipped);
    }

    [Fact(DisplayName = "Should not mark order as Shipped if it is not Preparing")]
    public void Should_Not_Mark_As_Shipped_When_Not_Preparing()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.PaymentConfirmed);

        // Act
        Action act = () => order.MarkAsShipped();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Order can only be marked as 'Shipped' after being in 'Preparing' status.");
    }

    [Fact(DisplayName = "Should mark order as delivered")]
    public void Should_Mark_As_Delivered()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Shipped);

        // Act
        order.MarkAsDelivered();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Delivered);
    }

    [Fact(DisplayName = "Should not mark order as Delivered if it is not Shipped")]
    public void Should_Not_Mark_As_Delivered_When_Not_Shipped()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, OrderStatus.Preparing);

        // Act
        Action act = () => order.MarkAsDelivered();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Order can only be marked as 'Delivered' after being 'Shipped'.");
    }

    // cancellation
    [Fact(DisplayName = "Should cancel a Pending order")]
    public void Should_Cancel_Pending_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 50m, 1);

        // Act
        order.CancelOrder();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Fact(DisplayName = "Should cancel a PaymentConfirmed order")]
    public void Should_Cancel_PaymentConfirmed_Order()
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        order.AddItem(ValidProductId, "Product", 50m, 1);
        var payment = order.StartPayment(PaymentMethod.Pix);
        order.HandleApprovedPayment(payment.Id); // Status: PaymentConfirmed

        // Act
        order.CancelOrder();

        // Assert
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Theory(DisplayName = "Should not allow canceling order after InPreparation")]
    [InlineData(OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    public void Should_Not_Cancel_After_InPreparation(OrderStatus status)
    {
        // Arrange
        var order = Order.Create(ValidCustomerId, CreateValidAddress());
        SetOrderStatus(order, status);

        // Act
        Action act = () => order.CancelOrder();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("It is not possible to cancel an order that is already in 'Preparing' status or beyond.");
    }
}
