using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Catalog.ValueObjects;


public sealed class ProductName : ValueObject
{
    public string Value { get; }

    public ProductName(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(value),
            "Product name is required.");

        Guard.Against<DomainException>(value.Length < 3,
            "Product name must be at least 3 characters long.");

        Guard.Against<DomainException>(value.Length > 150,
            "Product name must not exceed 150 characters.");

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
