using Company.Sales.Domain.Common.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Sales.Domain.Customers.Events;

public sealed record PrimaryAddressChangedEvent(
    Guid CustomerId,
    Guid NewPrimaryAddressId) : DomainEventBase;
