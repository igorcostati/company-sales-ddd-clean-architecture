using Company.Sales.Domain.Common.Base;
using Company.Sales.Domain.Common.Exceptions;
using Company.Sales.Domain.Common.Validations;

namespace Company.Sales.Domain.Customers.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Number { get; }

    public Cpf(string number)
    {
        Guard.AgainstNullOrWhiteSpace(number, nameof(number), "CPF is required.");

        var digits = new string(number.Where(char.IsDigit).ToArray());

        Guard.Against<DomainException>(digits.Length != 11, "CPF must contain 11 digits.");
        Guard.Against<DomainException>(!IsValidCpf(digits), "Invalid CPF.");

        Number = digits;
    }

    public override string ToString()
        => Convert.ToUInt64(Number).ToString(@"000\.000\.000\-00");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    private static bool IsValidCpf(string cpf)
    {
        // Reject repeated CPFs
        if (new string(cpf[0], cpf.Length) == cpf)
            return false;

        int Sum(int length, int weight)
        {
            int sum = 0;
            for (int i = 0; i < length; i++)
                sum += (cpf[i] - '0') * (weight - i);
            return sum;
        }

        int dv1 = Sum(9, 10) % 11;
        dv1 = dv1 < 2 ? 0 : 11 - dv1;

        int dv2 = Sum(10, 11) % 11;
        dv2 = dv2 < 2 ? 0 : 11 - dv2;

        return cpf[9] - '0' == dv1 && cpf[10] - '0' == dv2;
    }
}