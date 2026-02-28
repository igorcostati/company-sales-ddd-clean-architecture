using FluentAssertions;

namespace Company.Sales.Domain.Test;

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
        var state = "SP";
        var city = "São Paulo";
        var country = "Brazil";

        // Act
        var address = DeliveryAddress.Create(
            zipCode,
            street,
            complement,
            neighborhood,
            state,
            city,
            country);

        // Assert
        address.Should().NotBeNull();
        address.ZipCode.Should().Be(zipCode);
        address.Street.Should().Be(street);
        address.Complement.Should().Be(complement);
        address.Neighborhood.Should().Be(neighborhood);
        address.State.Should().Be(state);
        address.City.Should().Be(city);
        address.Country.Should().Be(country);
        address.FormatAddress().Should().Contain("Rua das Flores");
    }
}
