namespace MillifyDotnet;

internal static class Utilities
{
    internal static readonly string[] DefaultUnits = { string.Empty, "k", "m", "g", "t", "p", "e", "z", "y" };
    
    internal static void Validate<T>(this T actualValue, Func<T, bool> predicate, string message)
    {
        var isValid = predicate.Invoke(actualValue);

        if (!isValid) 
            throw new ArgumentException(message);
    }
}
