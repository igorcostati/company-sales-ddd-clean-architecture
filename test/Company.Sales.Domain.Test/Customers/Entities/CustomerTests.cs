using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Customers.Entities;
using Company.Sales.Domain.Customers.Enums;
using Company.Sales.Domain.Customers.Events;
using Company.Sales.Domain.Customers.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Sales.Domain.Test.Customers.Entities;

public class CustomerTests
{
    private static FullName CreateFullName(string name = "João Silva")
        => new(name);

    private static Cpf CreateCpf(string cpf = "12345678909")
        => new(cpf);

    private static Email CreateEmail(string email = "joao@example.com")
        => new(email);

    private static Phone CreatePhone(string phone = "11999999999")
        => new(phone);

    private static Address CreateAddress(
        string zipCode = "01310100",
        string street = "Avenida Paulista",
        string number = "1000",
        string district = "Bela Vista",
        string city = "São Paulo",
        string state = "SP",
        string country = "Brazil",
        string complement = "")
        => new(zipCode, street, number, district, city, state, country, complement);

    private static Customer CreateValidCustomer()
        => new Customer(
            CreateFullName(),
            CreateCpf(),
            CreateEmail(),
            CreatePhone(),
            CreateAddress(),
            Gender.Male,
            MaritalStatus.Single);

    [Fact]
    public void Constructor_WithValidData_ShouldCreateCustomer()
    {
        var customer = CreateValidCustomer();

        customer.Status.Should().Be(CustomerStatus.Active);
        customer.Gender.Should().Be(Gender.Male);
        customer.MaritalStatus.Should().Be(MaritalStatus.Single);

        customer.Addresses.Should().ContainSingle();
        customer.PrimaryAddressId.Should().Be(customer.Addresses.First().Id);
    }

    [Fact]
    public void Constructor_ShouldRaiseCustomerRegisteredEvent()
    {
        var customer = CreateValidCustomer();

        customer.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CustomerRegisteredEvent>();
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Cpf")]
    [InlineData("Email")]
    [InlineData("Phone")]
    [InlineData("Address")]
    public void Constructor_WithNullRequiredParameter_ShouldThrowDomainException(string field)
    {
        FullName? name = field == "Name" ? null : CreateFullName();
        Cpf? cpf = field == "Cpf" ? null : CreateCpf();
        Email? email = field == "Email" ? null : CreateEmail();
        Phone? phone = field == "Phone" ? null : CreatePhone();
        Address? address = field == "Address" ? null : CreateAddress();

        Action act = () => new Customer(name!, cpf!, email!, phone!, address!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddAddress_ShouldAddAddress()
    {
        var customer = CreateValidCustomer();
        var newAddress = CreateAddress("01452001", "Av. Brigadeiro Faria Lima");

        customer.AddAddress(newAddress);

        customer.Addresses.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddAddress_NullValidation(bool useNull)
    {
        var customer = CreateValidCustomer();
        Address? address = useNull ? null : CreateAddress();

        Action act = () => customer.AddAddress(address!);

        if (useNull)
            act.Should().Throw<DomainException>();
        else
            act.Should().NotThrow();
    }

    [Fact]
    public void AddAddress_ShouldUpdateModificationDate()
    {
        var customer = CreateValidCustomer();
        var previousDate = customer.UpdatedAt;

        System.Threading.Thread.Sleep(5);
        customer.AddAddress(CreateAddress("01452001", "Av. Brigadeiro Faria Lima"));

        customer.UpdatedAt.Should().BeAfter(previousDate);
    }

    [Fact]
    public void RemoveAddress_WithSecondAddress_ShouldRemove()
    {
        var customer = CreateValidCustomer();
        var second = CreateAddress("01452001", "Av. Brigadeiro Faria Lima");
        customer.AddAddress(second);

        customer.RemoveAddress(second.Id);

        customer.Addresses.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("NotExists")]
    [InlineData("Last")]
    public void RemoveAddress_ShouldThrowExceptions(string scenario)
    {
        var customer = CreateValidCustomer();

        Guid id = scenario switch
        {
            "NotExists" => Guid.NewGuid(),
            "Last" => customer.PrimaryAddressId,
            _ => throw new ArgumentOutOfRangeException()
        };

        Action act = () => customer.RemoveAddress(id);

        act.Should().Throw<DomainException>();
    }
}