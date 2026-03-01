using Company.Sales.Domain.Common.Exceptions;

namespace Company.Sales.Domain.Common.Validations;

internal static class Guard
{
    public static void AgainstEmptyGuid(Guid value, string parameterName, string? message = null)
    {
        if (value == Guid.Empty)
            throw new DomainException(message ?? $"The {parameterName} cannot be Guid.Empty.");
    }

    public static void AgainstNull<T>(T value, string parameterName)
    {
        if (value == null)
            throw new DomainException($"The {parameterName} cannot be null.");
    }

    public static void AgainstNullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
            throw new DomainException($"The {parameterName} cannot be null or empty.");
    }
    public static void AgainstNullOrWhiteSpace(string value, string parameterName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message ?? $"The {parameterName} cannot be null, empty or whitespace.");
    }

    public static void Against<TException>(bool condition, string message) where TException : Exception
    {
        if (condition)
            throw (TException)Activator.CreateInstance(typeof(TException), message)!;
    }
}
