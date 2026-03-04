using System.Text.RegularExpressions;
using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Customers.Entities;

public sealed class Address : Entity
{
    public string ZipCode { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string District { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string Complement { get; private set; }

    public Address(
        string zipCode,
        string street,
        string number,
        string district,
        string city,
        string state,
        string country,
        string complement = "")
    {
        Validate(zipCode, street, number, district, city, state, country);

        ZipCode = zipCode;
        Street = street;
        Number = number;
        District = district;
        City = city;
        State = state;
        Country = country;
        Complement = complement;
    }

    internal void Update(
        string zipCode,
        string street,
        string number,
        string district,
        string city,
        string state,
        string country,
        string complement = "")
    {
        Validate(zipCode, street, number, district, city, state, country);

        ZipCode = zipCode;
        Street = street;
        Number = number;
        District = district;
        City = city;
        State = state;
        Country = country;
        Complement = complement;
    }

    private static void Validate(
        string zipCode,
        string street,
        string number,
        string district,
        string city,
        string state,
        string country)
    {
        Guard.AgainstNullOrWhiteSpace(zipCode, nameof(zipCode), "Zip code is required.");
        Guard.Against<DomainException>(
            !Regex.IsMatch(zipCode, @"^\d{8}$"),
            "Invalid zip code.");

        Guard.AgainstNullOrWhiteSpace(street, nameof(street), "Street is required.");
        Guard.Against<DomainException>(
            street.Length < 3,
            "Street name is too short.");

        Guard.AgainstNullOrWhiteSpace(number, nameof(number), "Number is required.");
        Guard.Against<DomainException>(
            number.Length == 0,
            "Invalid number.");

        Guard.AgainstNullOrWhiteSpace(district, nameof(district), "District is required.");
        Guard.AgainstNullOrWhiteSpace(city, nameof(city), "City is required.");
        Guard.AgainstNullOrWhiteSpace(state, nameof(state), "State is required.");
        Guard.AgainstNullOrWhiteSpace(country, nameof(country), "Country is required.");
    }
}