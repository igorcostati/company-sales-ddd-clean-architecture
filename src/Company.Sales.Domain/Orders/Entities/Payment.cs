using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Enums;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;
using Company.Sales.Domain.Orders.Events;

namespace Company.Sales.Domain.Orders.Entities;

public sealed class Payment : Entity
{
    public Guid OrderId { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public PaymentStatus PaymentStatus { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime? PaymentDate { get; private set; }

    public string? TransactionCode { get; private set; }

    public Payment(Guid orderId, PaymentMethod paymentMethod, decimal amount)
    {
        Guard.AgainstEmptyGuid(orderId, nameof(orderId), "Invalid order.");
        Guard.Against<DomainException>(amount <= 0, "Payment amount must be greater than zero.");
        Guard.Against<DomainException>(
            !Enum.IsDefined(typeof(PaymentMethod), paymentMethod),
            "Invalid payment method."
        );

        OrderId = orderId;
        PaymentMethod = paymentMethod;
        Amount = amount;

        // Initial payment status
        PaymentStatus = PaymentStatus.Pending;
        PaymentDate = null;
        TransactionCode = null;
    }

    // As this project is only a demonstration of knowledge, I will not implement 
    // an integration with an external payment gateway to avoid increasing complexity. 
    // Instead, the method below generates a simulated transaction code locally.
    public void GenerateLocalTransactionCode()
    {
        if (TransactionCode is not null)
            return; // already generated

        var code = $"LOCAL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        SetTransactionCode(code);
    }
    public void SetTransactionCode(string code)
    {
        Guard.AgainstNullOrWhiteSpace(code, nameof(code), "Invalid transaction code.");
        Guard.Against<DomainException>(
            TransactionCode is not null,
            "Transaction code has already been generated."
        );

        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "It is not allowed to register a code after payment confirmation or rejection."
        );

        // Generated only once, when the payment is approved
        TransactionCode = code;
        SetUpdatedAt();
    }

    public void ConfirmPayment()
    {
        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "Only pending payments can be confirmed."
        );

        Guard.AgainstNullOrWhiteSpace(
            TransactionCode ?? string.Empty,
            nameof(TransactionCode),
            "Payment cannot be confirmed without a transaction code."
        );

        PaymentStatus = PaymentStatus.Approved;
        PaymentDate = DateTime.UtcNow;

        SetUpdatedAt();

        AddDomainEvent(new PaymentApprovedEvent(
            Id,
            OrderId,
            Amount,
            PaymentDate.Value,
            TransactionCode!
        ));
    }

    public void RejectPayment()
    {
        Guard.Against<DomainException>(
            PaymentStatus != PaymentStatus.Pending,
            "Only pending payments can be rejected."
        );

        PaymentStatus = PaymentStatus.Rejected;
        PaymentDate = DateTime.UtcNow;

        SetUpdatedAt();

        AddDomainEvent(new PaymentRejectedEvent(
            Id,
            OrderId,
            Amount,
            PaymentDate.Value,
            TransactionCode
        ));
    }
}
