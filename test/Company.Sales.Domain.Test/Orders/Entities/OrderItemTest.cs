using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Orders.Entities;
using FluentAssertions;

namespace Company.Sales.Domain.Test.Orders.Entities;

public class OrderItemTest
{
    // helper method
    private static OrderItem CreateValidItem(decimal price = 100m, int quantity = 2)
    {
        return new OrderItem(Guid.NewGuid(), "Test Product", price, quantity);
    }

    [Fact(DisplayName = "Should create OrderItem successfully when data is valid")]
    public void Create_ShouldReturnOrderItem_WhenDataIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = "Mechanical Keyboard";
        var unitPrice = 250m;
        var quantity = 2;

        // Act
        var item = new OrderItem(productId, productName, unitPrice, quantity);

        // Assert
        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be(productName);
        item.UnitPrice.Should().Be(unitPrice);
        item.Quantity.Should().Be(quantity);
        item.AppliedDiscount.Should().Be(0);
        item.TotalAmount.Should().Be(500m);
    }

    [Theory(DisplayName = "Should throw DomainException when parameters are invalid")]
    [InlineData("", "Product A", 10, 1, "Invalid ProductId.")]
    [InlineData("guid", "", 10, 1, "Product name is required.")]
    [InlineData("guid", "Product B", 0, 1, "Unit price must be greater than zero.")]
    [InlineData("guid", "Product C", 10, 0, "Quantity must be greater than zero.")]
    public void Create_ShouldThrowException_WhenParametersAreInvalid(
    string type,
    string productName,
    decimal price,
    int quantity,
    string message)
    {
        // Arrange
        var productId = type == "guid" ? Guid.NewGuid() : Guid.Empty;

        // Act
        Action act = () => new OrderItem(productId, productName, price, quantity);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage(message);
    }

    // --- DISCOUNT TESTS ---
    [Fact(DisplayName = "Should apply discount successfully when value is valid")]
    public void ApplyDiscount_ShouldApplySuccessfully_WhenValueIsValid()
    {
        // Arrange
        var item = CreateValidItem(price: 200m, quantity: 2);

        // Act
        item.ApplyDiscount(50m);

        // Assert
        item.AppliedDiscount.Should().Be(50m);
        item.TotalAmount.Should().Be(350m); // (200 * 2) - 50
        item.UpdatedAt.Should().NotBe(default(DateTime));
    }

    [Theory(DisplayName = "Should throw exception when applying invalid discount")]
    [InlineData(-10, "Discount cannot be negative.")]
    [InlineData(1000, "Discount cannot exceed the total item amount.")]
    public void ApplyDiscount_ShouldThrowException_WhenValueIsInvalid(
    decimal discount,
    string message)
    {
        // Arrange
        var item = CreateValidItem(price: 100m, quantity: 2);

        // Act
        Action act = () => item.ApplyDiscount(discount);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage(message);
    }

    // --- UNIT ADDITION TESTS ---
    [Fact(DisplayName = "Should add units successfully when value is valid")]
    public void AddUnits_ShouldAddSuccessfully_WhenValueIsValid()
    {
        // Arrange
        var item = CreateValidItem(price: 50m, quantity: 2);

        // Act
        item.AddUnits(3);

        // Assert
        item.Quantity.Should().Be(5);
        item.TotalAmount.Should().Be(250m); // 50 * 5
        item.UpdatedAt.Should().NotBe(default(DateTime));
    }

    [Fact(DisplayName = "Should throw exception when adding invalid units")]
    public void AddUnits_ShouldThrowException_WhenValueIsInvalid()
    {
        // Arrange
        var item = CreateValidItem();

        // Act
        Action act = () => item.AddUnits(0);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("At least one unit must be added.");
    }

    // --- UNIT REMOVAL TESTS ---
    [Fact(DisplayName = "Should remove units successfully when value is valid")]
    public void RemoveUnits_ShouldRemoveSuccessfully_WhenValueIsValid()
    {
        // Arrange
        var item = CreateValidItem(price: 100m, quantity: 5);

        // Act
        item.RemoveUnits(2);

        // Assert
        item.Quantity.Should().Be(3);
        item.TotalAmount.Should().Be(300m); // 100 * 3
        item.UpdatedAt.Should().NotBe(default(DateTime));
    }

    [Fact(DisplayName = "Should throw exception when updating unit price with invalid value")]
    public void UpdateUnitPrice_ShouldThrowException_WhenValueIsInvalid()
    {
        // Arrange
        var item = CreateValidItem();

        // Act
        Action act = () => item.UpdateUnitPrice(0);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Unit price must be greater than zero.");
    }

    // --- ENTITY EQUALITY TEST ---
    [Fact(DisplayName = "Two items with same Id should be considered equal")]
    public void Equals_ShouldReturnTrue_WhenSameId()
    {
        // Arrange
        var item1 = CreateValidItem();
        var item2 = CreateValidItem();

        // Force same Id via reflection
        typeof(Entity)
            .GetProperty("Id")!
            .SetValue(item2, item1.Id);

        // Act & Assert
        (item1 == item2).Should().BeTrue();
        item1.Equals(item2).Should().BeTrue();
    }
}
