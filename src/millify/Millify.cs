using System.Globalization;
using System.Numerics;

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
    /// Shortens a <see cref="BigInteger"/> into a more readable format using SI units.
    /// </summary>
    /// <param name="number">The number to be shortened.</param>
    /// <param name="options">The options to configure the shortening process. If null, default options will be used.</param>
    /// <returns>A string representing the shortened number with the appropriate unit.</returns>
    public static string Shorten(BigInteger number, MillifyOptions? options = null) =>
        FormatScaled(Decompose(number, options), options);

    /// <summary>
    /// Shortens a decimal number into a more readable format using SI units.
    /// </summary>
    /// <param name="number">The number to be shortened.</param>
    /// <param name="options">The options to configure the shortening process. If null, default options will be used.</param>
    /// <returns>A string representing the shortened number with the appropriate SI unit.</returns>
    public static string Shorten(decimal number, MillifyOptions? options = null) =>
        FormatScaled(Decompose(number, options), options);

    /// <summary>
    /// Decomposes a value into a scaled magnitude and unit index without applying final formatting rules.
    /// </summary>
    /// <param name="number">The input value.</param>
    /// <param name="options">Scaling options (scale base and units). When null, defaults are used.</param>
    public static MillifiedNumber Decompose(long number, MillifyOptions? options = null) =>
        Decompose(Convert.ToDecimal(number), options);

    /// <summary>
    /// Decomposes a value into a scaled magnitude and unit index without applying final formatting rules.
    /// </summary>
    /// <param name="number">The input value.</param>
    /// <param name="options">Scaling options (scale base and units). When null, defaults are used.</param>
    public static MillifiedNumber Decompose(double number, MillifyOptions? options = null) =>
        Decompose(Convert.ToDecimal(number), options);

    /// <summary>
    /// Decomposes a value into a scaled magnitude and unit index without applying final formatting rules.
    /// </summary>
    /// <param name="number">The input value.</param>
    /// <param name="options">Scaling options (scale base and units). When null, defaults are used.</param>
    public static MillifiedNumber Decompose(decimal number, MillifyOptions? options = null)
    {
        options ??= new MillifyOptions();
        return DecomposeDecimal(number, options);
    }

    /// <summary>
    /// Decomposes a <see cref="BigInteger"/> into a scaled magnitude and unit index without applying final formatting rules.
    /// </summary>
    /// <param name="number">The input value.</param>
    /// <param name="options">Scaling options (scale base and units). When null, defaults are used.</param>
    public static MillifiedNumber Decompose(BigInteger number, MillifyOptions? options = null)
    {
        options ??= new MillifyOptions();
        return DecomposeBigInteger(number, options);
    }

    /// <summary>
    /// Formats a previously decomposed value using the supplied options.
    /// </summary>
    /// <param name="value">The decomposed value.</param>
    /// <param name="options">Formatting options. When null, defaults are used.</param>
    public static string FormatScaled(MillifiedNumber value, MillifyOptions? options = null)
    {
        options ??= new MillifyOptions();
        return FormatScaledCore(value, options);
    }

    /// <summary>
    /// Writes the shortened representation of <paramref name="number"/> into <paramref name="destination"/>.
    /// </summary>
    /// <param name="number">The number to shorten.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <param name="charsWritten">The number of characters written when this method returns true.</param>
    /// <param name="options">Formatting options. When null, defaults are used.</param>
    /// <returns>False when <paramref name="destination"/> is too small to hold the full output.</returns>
    public static bool TryFormat(decimal number, Span<char> destination, out int charsWritten,
        MillifyOptions? options = null) =>
        TryFormatCore(Shorten(number, options), destination, out charsWritten);

    /// <summary>
    /// Writes the shortened representation of <paramref name="number"/> into <paramref name="destination"/>.
    /// </summary>
    public static bool TryFormat(long number, Span<char> destination, out int charsWritten,
        MillifyOptions? options = null) =>
        TryFormatCore(Shorten(number, options), destination, out charsWritten);

    /// <summary>
    /// Writes the shortened representation of <paramref name="number"/> into <paramref name="destination"/>.
    /// </summary>
    public static bool TryFormat(double number, Span<char> destination, out int charsWritten,
        MillifyOptions? options = null) =>
        TryFormatCore(Shorten(number, options), destination, out charsWritten);

    /// <summary>
    /// Writes the shortened representation of <paramref name="number"/> into <paramref name="destination"/>.
    /// </summary>
    public static bool TryFormat(BigInteger number, Span<char> destination, out int charsWritten,
        MillifyOptions? options = null) =>
        TryFormatCore(Shorten(number, options), destination, out charsWritten);

    private static bool TryFormatCore(string text, Span<char> destination, out int charsWritten)
    {
        if (text.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        text.AsSpan().CopyTo(destination);
        charsWritten = text.Length;
        return true;
    }

    private static MillifiedNumber DecomposeDecimal(decimal number, MillifyOptions options)
    {
        var divisor = (decimal)(int)options.ScaleBase;
        var isNegative = number < 0;
        var absoluteValue = Math.Abs(number);
        var unitIndex = 0;

        while (absoluteValue >= divisor && unitIndex < options.Units.Length - 1)
        {
            absoluteValue /= divisor;
            unitIndex++;
        }

        if (isNegative)
            absoluteValue *= -1m;

        return new MillifiedNumber(absoluteValue, unitIndex);
    }

    private static MillifiedNumber DecomposeBigInteger(BigInteger number, MillifyOptions options)
    {
        var divisor = (BigInteger)(int)options.ScaleBase;
        var isNegative = number < 0;
        var originalAbs = BigInteger.Abs(number);
        var unitIndex = 0;

        var probe = originalAbs;
        while (probe >= divisor && unitIndex < options.Units.Length - 1)
        {
            probe /= divisor;
            unitIndex++;
        }

        var denominator = BigInteger.Pow(divisor, unitIndex);
        decimal scaled;

        try
        {
            scaled = (decimal)originalAbs / (decimal)denominator;
        }
        catch (OverflowException)
        {
            var numeratorDouble = (double)originalAbs;
            var denominatorDouble = Math.Pow((double)(int)options.ScaleBase, unitIndex);

            if (double.IsInfinity(numeratorDouble) || double.IsNaN(numeratorDouble) ||
                double.IsInfinity(denominatorDouble) || denominatorDouble == 0d)
            {
                throw new OverflowException(
                    "The scaled magnitude is too large to represent. Add more unit entries so the value can be scaled into a representable range.");
            }

            scaled = (decimal)(numeratorDouble / denominatorDouble);
        }

        if (isNegative)
            scaled *= -1m;

        return new MillifiedNumber(scaled, unitIndex);
    }

    private static string FormatScaledCore(MillifiedNumber value, MillifyOptions options)
    {
        var isNegative = value.ScaledValue < 0;
        var absoluteValue = Math.Abs(value.ScaledValue);

        var effectivePrecision = GetEffectivePrecision(absoluteValue, options.Precision, options.SmartPrecision);

        var numberFormat = CreateDecimalOnlyNumberFormat(options.Culture);
        var formattedNumber = absoluteValue.ToString($"F{effectivePrecision}", numberFormat);

        if (options.TrimInsignificantZeros)
            formattedNumber = TrimTrailingZeros(formattedNumber, numberFormat.NumberDecimalSeparator);

        if (isNegative)
            formattedNumber = $"-{formattedNumber}";

        var unit = options.Units[value.UnitIndex];
        var casedUnit = CaseSuffix(unit, options.Lowercase, options.ScaleBase);

        return options.SpaceBeforeUnit
            ? $"{formattedNumber} {casedUnit}"
            : $"{formattedNumber}{casedUnit}";
    }

    private static int GetEffectivePrecision(decimal absoluteScaledValue, int maxPrecision, bool smart)
    {
        if (!smart)
            return maxPrecision;

        if (absoluteScaledValue >= 100m)
            return 0;

        if (absoluteScaledValue >= 10m)
            return Math.Min(1, maxPrecision);

        if (absoluteScaledValue >= 1m)
            return maxPrecision;

        return maxPrecision;
    }

    private static NumberFormatInfo CreateDecimalOnlyNumberFormat(CultureInfo? culture)
    {
        var sourceCulture = culture ?? CultureInfo.InvariantCulture;
        var nfi = (NumberFormatInfo)sourceCulture.NumberFormat.Clone();
        nfi.NumberGroupSizes = [0];
        return nfi;
    }

    private static string CaseSuffix(string unit, bool lowercase, MillifyScaleBase scaleBase)
    {
        if (lowercase)
            return unit.ToLowerInvariant();

        if (scaleBase == MillifyScaleBase.Binary)
        {
            var u = unit.ToLowerInvariant();
            if (u.Length >= 2 && u[u.Length - 1] == 'i')
                return char.ToUpperInvariant(u[0]) + u.Substring(1);

            return unit.ToUpperInvariant();
        }

        return unit.ToUpperInvariant();
    }

    private static string TrimTrailingZeros(string formattedNumber, string decimalSeparator)
    {
        if (decimalSeparator.Length == 0)
            return formattedNumber;

        var sepIndex = formattedNumber.IndexOf(decimalSeparator, StringComparison.Ordinal);
        if (sepIndex < 0)
            return formattedNumber;

        var fracStart = sepIndex + decimalSeparator.Length;
        var i = formattedNumber.Length - 1;
        while (i >= fracStart && formattedNumber[i] == '0')
            i--;

        if (i < fracStart)
            return formattedNumber.Substring(0, sepIndex);

        return formattedNumber.Substring(0, i + 1);
    }
}
