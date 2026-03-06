using Company.Sales.Domain.Catalog.Events;
using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Catalog.Entities;


public sealed class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public Category(string name, string? description = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name), "Name is required.");
        Guard.Against<DomainException>(name.Length < 3, "Name must be at least 3 characters long.");

        Name = name.Trim();
        Description = description;
        IsActive = true;
    }

    public void ChangeName(string newName)
    {
        Guard.AgainstNullOrWhiteSpace(newName, nameof(newName), "Name is required.");
        Guard.Against<DomainException>(newName.Length < 3, "Name must be at least 3 characters long.");

        Name = newName.Trim();
        SetUpdatedAt();
    }

    public void ChangeDescription(string? newDescription)
    {
        Description = newDescription;
        SetUpdatedAt();
    }

    public void Activate()
    {
        Guard.Against<DomainException>(IsActive, "Category is already active.");

        IsActive = true;
        SetUpdatedAt();
        AddDomainEvent(new CategoryActivatedEvent(Id));
    }

    public void Deactivate()
    {
        Guard.Against<DomainException>(!IsActive, "Category is already inactive.");

        IsActive = false;
        SetUpdatedAt();
        AddDomainEvent(new CategoryDeactivatedEvent(Id));
    }
}
