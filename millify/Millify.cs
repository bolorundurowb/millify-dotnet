namespace MillifyDotnet;

public static class Millify
{
    private static readonly string[] DefaultUnits = { "k", "m", "g", "t", "p", "e", "z", "y" };

    public static string Shorten(double number, MillifyOptions? options = null)
    {
        options ??= new();
        var isNegative = number < 0;
        var units = options.Units ?? DefaultUnits;

        var absoluteValue = Math.Abs(number);
        var unitIndex = 0;

        while (absoluteValue >= 1000 && unitIndex < units.Length - 1)
        {
            absoluteValue /= 1000;
            unitIndex++;
        }

        if (isNegative)
            absoluteValue *= -1;

        var formattedNumber = absoluteValue.ToString($"F{options.Precision}");

        if (options.Lowercase)
            formattedNumber = formattedNumber.ToUpper();

        var unit = units[unitIndex];
        var casedUnit = options.Lowercase
            ? unit.ToLowerInvariant()
            : unit.ToUpperInvariant();

        return options.SpaceBeforeUnit
            ? $"{formattedNumber} {casedUnit}"
            : $"{formattedNumber}{casedUnit}";
    }
}
