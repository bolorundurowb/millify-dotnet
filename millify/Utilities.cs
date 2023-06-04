namespace MillifyDotnet;

internal static class Utilities
{
    public static void Validate<T>(this T actualValue, Func<T, bool> predicate, string message)
    {
        var isValid = predicate.Invoke(actualValue);

        if (!isValid) 
            throw new ArgumentException(message);
    }
}
