using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Company.Sales.Domain.Customers.ValueObjects;

public sealed class Email : ValueObject
{
    public string Address { get; }

    private static readonly Regex _regex = new(
        @"^[\w\.-]+@[\w\.-]+\.\w{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string address)
    {
        Guard.AgainstNullOrWhiteSpace(address, nameof(address), "Email is required.");
        Guard.Against<DomainException>(
            !_regex.IsMatch(address),
            "Invalid email.");

        Address = address.Trim().ToLowerInvariant();
    }

    public override string ToString() => Address;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }
}
