using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Catalog.ValueObjects;

public sealed class ProductImage : ValueObject
{
    public string Url { get; }
    public int Order { get; }

    public ProductImage(string url, int order)
    {
        Guard.AgainstNullOrWhiteSpace(url, nameof(url),
            "Image URL is required.");

        Guard.Against<DomainException>(order < 1,
            "Image order must be greater than or equal to 1.");

        Url = url;
        Order = order;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
        yield return Order;
    }
}
