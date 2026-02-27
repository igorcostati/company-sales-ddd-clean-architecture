namespace Company.Sales.Domain;

internal static class Guard
{
    public static void AgainstEmptyGuid(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
            throw new DomainException($"The {parameterName} cannot be Guid.Empty.");
    }

    public static void AgainstNull<T>(T value, string parameterName)
    {
        if (value == null)
            throw new DomainException($"The {parameterName} cannot be null.");
    }
    public static void Against<TException>(bool condition, string message) where TException : Exception
    {
        if (condition)
            throw (TException)Activator.CreateInstance(typeof(TException), message)!;
    }
}
