using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Orders.ValuesObjects;
using FluentAssertions;

namespace Company.Sales.Domain.Test.Orders.ValueObjects;

public class DeliveryAddressTest
{
    [Fact(DisplayName = "Create should return valid address when data is valid")]
    public void Create_ShouldReturnValidAddress_WhenDataIsValid()
    {
        // Arrange
        var zipCode = "12345-678";
        var street = "Rua das Flores";
        var complement = "Apto 101";
        var neighborhood = "Centro";
        var city = "São Paulo";
        var state = "SP";
        var country = "Brazil";

        // Act
        var address = DeliveryAddress.Create(
            zipCode,
            street,
            complement,
            neighborhood,
            city,
            state,
            country);

        // Assert
        address.Should().NotBeNull();
        address.ZipCode.Should().Be(zipCode);
        address.Street.Should().Be(street);
        address.Complement.Should().Be(complement);
        address.Neighborhood.Should().Be(neighborhood);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.Country.Should().Be(country);
        address.FormatAddress().Should().Contain("Rua das Flores");
    }


    [Theory(DisplayName = "Should throw DomainException when ZipCode is invalid")]
    [InlineData("12345678")]   // without hyphen
    [InlineData("12-345678")]  // incorrect format
    [InlineData("ABCDE-123")]  // invalid characters
    public void Create_ShouldThrowDomainException_WhenZipCodeIsInvalid(string invalidZipCode)
    {
        // Arrange
        var street = "Rua das Flores";
        var complement = "Casa";
        var neighborhood = "Centro";
        var city = "São Paulo";
        var state = "SP";
        var country = "Brazil";

        // Act
        Action act = () => DeliveryAddress.Create(
            invalidZipCode,
            street,
            complement,
            neighborhood,
            city,
            state,
            country);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Invalid zip code format*");
    }

    [Fact(DisplayName = "Two DeliveryAddresses with same data should be equal (Value Object)")]
    public void Addresses_ShouldBeEqual_WhenTheyHaveSameValues()
    {
        // Arrange
        var address1 = DeliveryAddress.Create(
            "12345-678",
            "Rua X",
            "Casa",
            "Centro",
            "SP",
            "São Paulo",
            "Brazil");

        var address2 = DeliveryAddress.Create(
            "12345-678",
            "Rua X",
            "Casa",
            "Centro",
            "SP",
            "São Paulo",
            "Brazil");

        // Assert
        address1.Should().Be(address2);
        (address1 == address2).Should().BeTrue();
    }

    [Fact(DisplayName = "DeliveryAddress should be immutable after creation")]
    public void Address_ShouldBeImmutable_AfterCreation()
    {
        // Arrange
        var address = DeliveryAddress.Create(
            "12345-678",
            "Rua X",
            "Casa",
            "Centro",
            "SP",
            "São Paulo",
            "Brazil");

        // Act (conceptual attempt – does not compile)
        Action act = () =>
        {
            // address.ZipCode = "99999-999";
        };

        // Assert
        address.GetType()
               .GetProperties()
               .All(p => p.SetMethod == null || p.SetMethod.IsPrivate)
               .Should()
               .BeTrue("Value Object properties must be immutable");
    }

#pragma warning disable xUnit1012
    [Theory(DisplayName = "Should throw DomainException when required fields are null or empty")]
    [InlineData(null, "Street", "Neighborhood", "State", "City", "Country")]               // ZipCode null
    [InlineData("12345-678", null, "Neighborhood", "State", "City", "Country")]            // Street null
    [InlineData("12345-678", "Street", "Neighborhood", "State", "City", null)]             // Country null
    public void Create_ShouldThrowDomainException_WhenRequiredFieldsAreNullOrEmpty(
    string zipCode,
    string street,
    string neighborhood,
    string state,
    string city,
    string country)
    {
        // Act
        Action act = () => DeliveryAddress.Create(
            zipCode,
            street,
            "Complement",
            neighborhood,
            state,
            city,
            country);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("*cannot be null or empty*");
    }
#pragma warning restore xUnit1012

}