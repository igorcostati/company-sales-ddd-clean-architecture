using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Orders.Entities;

public sealed class OrderItem : Entity
{
    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal AppliedDiscount { get; private set; }

    public decimal TotalAmount { get; private set; }

    internal OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Guard.AgainstEmptyGuid(productId, nameof(productId), "Invalid ProductId.");
        Guard.AgainstNullOrWhiteSpace(productName, nameof(productName), "Product name is required.");
        Guard.Against<DomainException>(unitPrice <= 0, "Unit price must be greater than zero.");
        Guard.Against<DomainException>(quantity <= 0, "Quantity must be greater than zero.");

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        AppliedDiscount = 0;

        CalculateTotalAmount();
    }

    public void ApplyDiscount(decimal discount)
    {
        Guard.Against<DomainException>(discount < 0, "Discount cannot be negative.");
        Guard.Against<DomainException>(
            discount > UnitPrice * Quantity,
            "Discount cannot exceed the total item amount."
        );

        AppliedDiscount = discount;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void AddUnits(int units)
    {
        Guard.Against<DomainException>(units <= 0, "At least one unit must be added.");

        Quantity += units;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void RemoveUnits(int units)
    {
        Guard.Against<DomainException>(units <= 0, "At least one unit must be removed.");
        Guard.Against<DomainException>(
            units > Quantity,
            "Cannot remove more units than currently exist in the item."
        );

        Quantity -= units;

        Guard.Against<DomainException>(
            Quantity == 0,
            "An order item cannot have zero quantity. Use the Order method to remove it."
        );

        SetUpdatedAt();
        CalculateTotalAmount();
    }

    public void UpdateUnitPrice(decimal newPrice)
    {
        Guard.Against<DomainException>(newPrice <= 0, "Unit price must be greater than zero.");

        UnitPrice = newPrice;
        SetUpdatedAt();
        CalculateTotalAmount();
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = (UnitPrice * Quantity) - AppliedDiscount;
    }

}
