using Company.Sales.Domain.Common.Enums;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Orders.Entities;
using Company.Sales.Domain.Orders.Events;
using FluentAssertions;

namespace Company.Sales.Domain.Test.Orders.Entities;

public class PaymentTest
{
    [Fact(DisplayName = "Should create a valid payment with pending status")]
    public void Should_Create_Valid_Payment_With_Pending_Status()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var paymentMethod = PaymentMethod.CreditCard;
        var amount = 100m;

        // Act
        var payment = new Payment(orderId, paymentMethod, amount);

        // Assert
        payment.OrderId.Should().Be(orderId);
        payment.PaymentMethod.Should().Be(paymentMethod);
        payment.Amount.Should().Be(amount);
        payment.PaymentStatus.Should().Be(PaymentStatus.Pending);
        payment.PaymentDate.Should().BeNull();
        payment.TransactionCode.Should().BeNull();
    }

    [Fact(DisplayName = "Should not create payment with amount less than or equal to zero")]
    public void Should_Not_Create_Payment_With_Invalid_Amount()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        Action act = () => new Payment(orderId, PaymentMethod.Pix, 0);

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Payment amount must be greater than zero.");
    }

    [Fact(DisplayName = "Should not set null or empty transaction code")]
    public void Should_Not_Set_Null_Or_Empty_Transaction_Code()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);

        // Act
        Action act = () => payment.SetTransactionCode("");

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Invalid transaction code.");
    }

    [Fact(DisplayName = "Should set valid transaction code and update UpdatedAt")]
    public void Should_Set_Valid_Transaction_Code()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 100m);
        var code = "TXN-12345";

        // Act
        payment.SetTransactionCode(code);

        // Assert
        payment.TransactionCode.Should().Be(code);
        payment.UpdatedAt.Should().NotBe(default(DateTime));
    }

    [Fact(DisplayName = "Should not redefine an already defined transaction code")]
    public void Should_Not_Redefine_Transaction_Code()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 100m);
        payment.SetTransactionCode("TXN-001");

        // Act
        Action act = () => payment.SetTransactionCode("TXN-002");

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Transaction code has already been generated.");
    }
    [Fact(DisplayName = "Should generate local transaction code automatically")]
    public void Should_Generate_Local_Transaction_Code()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 200m);

        // Act
        payment.GenerateLocalTransactionCode();

        // Assert
        payment.TransactionCode.Should().StartWith("LOCAL-");
        payment.TransactionCode.Should().HaveLength(14); // LOCAL- + 8 chars
        payment.UpdatedAt.Should().NotBe(default(DateTime));
    }

    [Fact(DisplayName = "Should confirm pending payment with valid code and generate complete event")]
    public void Should_Confirm_Payment_With_Valid_Code_And_Generate_Event()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.CreditCard, 300m);
        payment.GenerateLocalTransactionCode(); // Simulates gateway

        // Act
        payment.ConfirmPayment();

        // Assert
        payment.PaymentStatus.Should().Be(PaymentStatus.Approved);
        payment.PaymentDate.Should().NotBeNull();
        payment.UpdatedAt.Should().NotBe(default(DateTime));

        var approvedEvent = payment.DomainEvents
                            .OfType<PaymentApprovedEvent>()
                            .FirstOrDefault();

        approvedEvent.Should().NotBeNull();
        approvedEvent!.PaymentId.Should().Be(payment.Id);
        approvedEvent.OrderId.Should().Be(payment.OrderId);
        approvedEvent.Amount.Should().Be(payment.Amount);
        approvedEvent.TransactionCode.Should().Be(payment.TransactionCode);
        approvedEvent.PaymentDate.Should().Be(payment.PaymentDate);
    }

    [Fact(DisplayName = "Should not confirm payment without transaction code")]
    public void Should_Not_Confirm_Without_Transaction_Code()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);

        // Act
        Action act = () => payment.ConfirmPayment();

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Payment cannot be confirmed without a transaction code.");
    }

    [Fact(DisplayName = "Should not confirm payment that is not pending")]
    public void Should_Not_Confirm_Payment_When_Not_Pending()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 100m);
        payment.GenerateLocalTransactionCode();
        payment.ConfirmPayment(); // Now status is Approved

        // Act
        Action act = () => payment.ConfirmPayment();

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Only pending payments can be confirmed.");
    }

    [Fact(DisplayName = "Should reject pending payment and generate rejection event with correct data")]
    public void Should_Reject_Payment_And_Generate_Rejection_Event()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 120m);

        // Act
        payment.RejectPayment();

        // Assert
        payment.PaymentStatus.Should().Be(PaymentStatus.Rejected);
        payment.PaymentDate.Should().NotBeNull();
        payment.UpdatedAt.Should().NotBe(default(DateTime));

        var rejectedEvent = payment.DomainEvents
            .OfType<PaymentRejectedEvent>()
            .FirstOrDefault();

        rejectedEvent.Should().NotBeNull();
        rejectedEvent!.PaymentId.Should().Be(payment.Id);
        rejectedEvent.OrderId.Should().Be(payment.OrderId);
        rejectedEvent.Amount.Should().Be(payment.Amount);
        rejectedEvent.TransactionCode.Should().Be(payment.TransactionCode);
        rejectedEvent.PaymentDate.Should().Be(payment.PaymentDate);
    }

    [Fact(DisplayName = "Should not reject payment that is not pending")]
    public void Should_Not_Reject_Payment_When_Not_Pending()
    {
        // Arrange
        var payment = new Payment(Guid.NewGuid(), PaymentMethod.Pix, 120m);
        payment.GenerateLocalTransactionCode();
        payment.ConfirmPayment(); // Status is now Approved

        // Act
        Action act = () => payment.RejectPayment();

        // Assert
        act.Should()
           .Throw<DomainException>()
           .WithMessage("Only pending payments can be rejected.");
    }

}
