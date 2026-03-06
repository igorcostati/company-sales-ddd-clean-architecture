using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Catalog.ValueObjects;
public sealed class ProductCode : ValueObject
{
    public string Value { get; }

    public ProductCode(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(value),
            "Product code is required.");

        Guard.Against<DomainException>(value.Length < 3,
            "Product code must be at least 3 characters long.");

        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}