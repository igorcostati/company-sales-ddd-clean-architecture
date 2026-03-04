using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;

namespace Company.Sales.Domain.Orders.ValuesObjects;

public sealed class CancellationReason : ValueObject
{
    public string Code { get; }
    public string Description { get; }

    // Standardized reason set within the domain
    private static readonly Dictionary<string, string> _defaultReasons = new()
    {
        { "CustomerCancelled", "Customer cancelled the purchase" },
        { "PaymentError", "Error processing the payment" },
        { "OutOfStock", "Item out of stock" },
        { "InvalidAddress", "Invalid delivery address" },
        { "Other", "Other unspecified reason" }
    };

    // Constructor
    public CancellationReason(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Cancellation reason code is required.");

        if (!_defaultReasons.ContainsKey(code))
            throw new DomainException($"Cancellation reason '{code}' is not valid.");

        Code = code;
        Description = _defaultReasons[code];
    }

    // Factory methods for common reasons
    public static CancellationReason CustomerCancelled() => new("CustomerCancelled");
    public static CancellationReason PaymentError() => new("PaymentError");
    public static CancellationReason OutOfStock() => new("OutOfStock");
    public static CancellationReason InvalidAddress() => new("InvalidAddress");
    public static CancellationReason Other() => new("Other");
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Description;
    }

    public override string ToString() => $"{Code}: {Description}";
}
