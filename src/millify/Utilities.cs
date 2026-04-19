namespace MillifyDotnet;

internal static class Utilities
{
    internal static readonly string[] DefaultUnits = { string.Empty, "k", "m", "g", "t", "p", "e", "z", "y" };

    internal static readonly string[] DefaultBinaryUnits =
        { string.Empty, "ki", "mi", "gi", "ti", "pi", "ei", "zi", "yi" };

    internal static void Validate<T>(this T actualValue, Func<T, bool> predicate, string message)
    {
        var isValid = predicate.Invoke(actualValue);

        if (!isValid)
            throw new ArgumentException(message);
    }

    internal static void ValidateUnits(string[] units, string paramName)
    {
        if (units.Length == 0)
            throw new ArgumentException("Units must contain at least one entry.", paramName);

        for (var i = 0; i < units.Length; i++)
        {
            if (units[i] is null)
                throw new ArgumentException($"Units[{i}] must not be null.", paramName);
        }
    }
}
