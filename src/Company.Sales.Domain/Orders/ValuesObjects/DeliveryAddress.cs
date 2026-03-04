using System.Text.RegularExpressions;
using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Orders.ValuesObjects;

public class DeliveryAddress : ValueObject
{
    public string ZipCode { get; private set; }
    public string Street { get; private set; }
    public string Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public string State { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }

    private DeliveryAddress(string zipCode, string street, string complement
    , string neighborhood, string city, string state, string country)
    {
        Guard.AgainstNullOrEmpty(zipCode, nameof(zipCode));
        Guard.AgainstNullOrEmpty(street, nameof(street));
        Guard.AgainstNullOrEmpty(neighborhood, nameof(neighborhood));
        Guard.AgainstNullOrEmpty(city, nameof(city));
        Guard.AgainstNullOrEmpty(state, nameof(state));
        Guard.AgainstNullOrEmpty(country, nameof(country));

        if (!Regex.IsMatch(zipCode ?? "", @"^\d{5}-\d{3}$"))
            throw new DomainException("Invalid zip code format. Expected format: 12345-678");

        ZipCode = zipCode!;
        Street = street;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
    }

    public static DeliveryAddress Create(
          string zipCode
        , string street
        , string complement
        , string neighborhood
        , string city
        , string state
        , string country)
    {
        return new DeliveryAddress(zipCode, street, complement, neighborhood, city, state, country);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return (nameof(ZipCode), ZipCode);
        yield return (nameof(Street), Street);
        yield return Complement ?? string.Empty;
        yield return (nameof(Neighborhood), Neighborhood);
        yield return (nameof(City), City);
        yield return (nameof(State), State);
        yield return (nameof(Country), Country);
    }

    public string FormatAddress()
    {
        return $"{Street}, {Neighborhood}, {City} - {State}, {ZipCode}, {Country}";
    }
}
