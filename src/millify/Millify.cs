namespace MillifyDotnet;

/// <summary>
/// Provides methods to shorten large numbers into a more readable format using SI units.
/// </summary>
public static class Millify
{
    /// <summary>
    /// Shortens a long integer into a more readable format using SI units.
    /// </summary>
    /// <param name="number">The number to be shortened.</param>
    /// <param name="options">The options to configure the shortening process. If null, default options will be used.</param>
    /// <returns>A string representing the shortened number with the appropriate SI unit.</returns>
    public static string Shorten(long number, MillifyOptions? options = null) =>
        Shorten(Convert.ToDecimal(number), options);

    /// <summary>
    /// Shortens a double into a more readable format using SI units.
    /// </summary>
    /// <param name="number">The number to be shortened.</param>
    /// <param name="options">The options to configure the shortening process. If null, default options will be used.</param>
    /// <returns>A string representing the shortened number with the appropriate SI unit.</returns>
    public static string Shorten(double number, MillifyOptions? options = null) =>
        Shorten(Convert.ToDecimal(number), options);

    /// <summary>
    /// Shortens a decimal number into a more readable format using SI units.
    /// </summary>
    /// <param name="number">The number to be shortened.</param>
    /// <param name="options">The options to configure the shortening process. If null, default options will be used.</param>
    /// <returns>A string representing the shortened number with the appropriate SI unit.</returns>
    public static string Shorten(decimal number, MillifyOptions? options = null)
    {
        options ??= new MillifyOptions();
        var isNegative = number < 0;

        var absoluteValue = Math.Abs(number);
        var unitIndex = 0;

        while (absoluteValue >= 1000 && unitIndex < options.Units.Length - 1)
        {
            absoluteValue /= 1000;
            unitIndex++;
        }

        if (isNegative)
            absoluteValue *= -1;

        var formattedNumber = absoluteValue.ToString($"F{options.Precision}");

        if (options.Lowercase)
            formattedNumber = formattedNumber.ToUpper();

        var unit = options.Units[unitIndex];
        var casedUnit = options.Lowercase
            ? unit.ToLowerInvariant()
            : unit.ToUpperInvariant();

        return options.SpaceBeforeUnit
            ? $"{formattedNumber} {casedUnit}"
            : $"{formattedNumber}{casedUnit}";
    }
}
