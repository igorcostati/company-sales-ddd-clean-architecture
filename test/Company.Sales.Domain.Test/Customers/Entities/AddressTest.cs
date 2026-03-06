using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Customers.Entities;
using FluentAssertions;

namespace Company.Sales.Domain.Test.Customers.Entities;

public class AddressTests
{
    private static Address CreateValidAddress()
    {
        return new Address(
            zipCode: "12345678",
            street: "Street A",
            number: "100",
            district: "Downtown",
            city: "São Paulo",
            state: "SP",
            country: "Brazil"
        );
    }

    [Fact]
    public void Should_Create_Valid_Address()
    {
        // Arrange & Act
        var address = CreateValidAddress();

        // Assert
        address.ZipCode.Should().Be("12345678");
        address.Street.Should().Be("Street A");
        address.Number.Should().Be("100");
        address.District.Should().Be("Downtown");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
        address.Country.Should().Be("Brazil");
        address.Complement.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_Error_When_ZipCode_Is_Invalid(string? invalidZipCode)
    {
        // Arrange
        Action act = () => new Address(
            zipCode: invalidZipCode!,
            street: "Street A",
            number: "100",
            district: "Downtown",
            city: "São Paulo",
            state: "SP",
            country: "Brazil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Zip code is required.");
    }

    [Fact]
    public void Should_Throw_Error_When_ZipCode_Is_Not_8_Digits()
    {
        // Arrange
        Action act = () => new Address(
            zipCode: "1234",
            street: "Street A",
            number: "100",
            district: "Downtown",
            city: "São Paulo",
            state: "SP",
            country: "Brazil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid zip code.");
    }

    [Theory]
    [InlineData(null, "100", "Downtown", "São Paulo", "SP", "Brazil")]
    [InlineData("Street A", null, "Downtown", "São Paulo", "SP", "Brazil")]
    [InlineData("Street A", "100", null, "São Paulo", "SP", "Brazil")]
    [InlineData("Street A", "100", "Downtown", null, "SP", "Brazil")]
    [InlineData("Street A", "100", "Downtown", "São Paulo", null, "Brazil")]
    [InlineData("Street A", "100", "Downtown", "São Paulo", "SP", null)]
    public void Should_Throw_Error_When_Required_Fields_Are_Invalid(
        string? street,
        string? number,
        string? district,
        string? city,
        string? state,
        string? country)
    {
        // Arrange
        Action act = () => new Address(
            zipCode: "12345678",
            street: street!,
            number: number!,
            district: district!,
            city: city!,
            state: state!,
            country: country!
        );

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_Update_Address_With_Valid_Data()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        address.Update(
            zipCode: "87654321",
            street: "Street B",
            number: "200",
            district: "New District",
            city: "Rio de Janeiro",
            state: "RJ",
            country: "Brazil",
            complement: "Apt 12"
        );

        // Assert
        address.ZipCode.Should().Be("87654321");
        address.Street.Should().Be("Street B");
        address.Number.Should().Be("200");
        address.District.Should().Be("New District");
        address.City.Should().Be("Rio de Janeiro");
        address.State.Should().Be("RJ");
        address.Country.Should().Be("Brazil");
        address.Complement.Should().Be("Apt 12");
    }

    [Fact]
    public void Should_Throw_Error_When_Updating_With_Invalid_ZipCode()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        Action act = () => address.Update(
            zipCode: "123",
            street: "Test Street",
            number: "10",
            district: "Downtown",
            city: "SP",
            state: "SP",
            country: "Brazil"
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid zip code.");
    }

    [Fact]
    public void Should_Throw_Error_When_Updating_With_Invalid_Required_Field()
    {
        // Arrange
        var address = CreateValidAddress();

        // Act
        Action act = () => address.Update(
            zipCode: "12345678",
            street: "",
            number: "10",
            district: "Downtown",
            city: "SP",
            state: "SP",
            country: "Brazil"
        );

        // Assert
        act.Should().Throw<DomainException>();
    }
}
