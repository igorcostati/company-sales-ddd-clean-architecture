using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Catalog.ValueObjects;


public sealed class ProductPrice : ValueObject
{
    public decimal Value { get; }

    public ProductPrice(decimal value)
    {
        Guard.Against<DomainException>(value <= 0,
            "Product price must be greater than zero.");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
