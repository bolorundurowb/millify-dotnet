using System.Globalization;
using System.Numerics;

namespace MillifyDotnet.Tests;

public class MillifyFeaturesTests
{
    [Fact]
    public void Shorten_WithTrimZerosEnabled_RemovesTrailingFractionalZeros()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: true);
        Millify.Shorten(2500, options).Should().Be("2.5K");
    }

    [Fact]
    public void Shorten_WithTrimZerosDisabled_PreservesTrailingFractionalZeros()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: false);
        Millify.Shorten(2500, options).Should().Be("2.50K");
    }

    [Fact]
    public void Shorten_WithSmartPrecision_AdjustsFractionalPlacesByMagnitude()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(9990, options).Should().Be("9.99K");
        Millify.Shorten(125_000_000, options).Should().Be("125M");
    }

    [Fact]
    public void Shorten_WithBinaryScaling_UsesIecSuffixesAndBase1024()
    {
        var options = new MillifyOptions(precision: 2, scaleBase: MillifyScaleBase.Binary);
        Millify.Shorten(1024, options).Should().Be("1Ki");
        Millify.Shorten(1048576, options).Should().Be("1Mi");
    }

    [Fact]
    public void Shorten_WithBigIntegerLargeLiteral_ScalesIntoExaRangeWithTwoDecimals()
    {
        var value = BigInteger.Parse("12345678901234567890");
        var options = new MillifyOptions(precision: 2);
        Millify.Shorten(value, options).Should().Be("12.35E");
    }

    [Fact]
    public void Shorten_WithBigIntegerSmallValue_PreservesFractionalThousandsStep()
    {
        var options = new MillifyOptions(precision: 1);
        Millify.Shorten(new BigInteger(1234), options).Should().Be("1.2K");
    }

    [Fact]
    public void Shorten_WithFrenchCulture_UsesCommaAsDecimalSeparator()
    {
        var culture = new CultureInfo("fr-FR");
        var options = new MillifyOptions(precision: 1, culture: culture);
        Millify.Shorten(1234.5m, options).Should().Be("1,2K");
    }

    [Fact]
    public void FormatScaled_WithDecomposedValueAndGermanCulture_AllowsRoundTripWithShorten()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true, culture: new CultureInfo("de-DE"));
        var parts = Millify.Decompose(9990m, options);
        parts.ScaledValue.Should().Be(9.99m);
        parts.UnitIndex.Should().Be(1);
        Millify.FormatScaled(parts, options).Should().Be(Millify.Shorten(9990m, options));
    }

    [Fact]
    public void MillifiedNumber_ToFormattedString_AgreesWithShorten()
    {
        var options = new MillifyOptions(precision: 2);
        var parts = Millify.Decompose(2500m, options);
        parts.ToFormattedString(options).Should().Be(Millify.Shorten(2500m, options));
    }

    [Fact]
    public void TryFormat_WithSufficientBuffer_WritesCompleteAbbreviation()
    {
        Span<char> buffer = stackalloc char[32];
        Millify.TryFormat(1234.5m, buffer, out var written).Should().BeTrue();
        written.Should().Be(4);
        buffer.Slice(0, written).ToString().Should().Be("1.2K");
    }

    [Fact]
    public void TryFormat_WithInsufficientBuffer_ReturnsFalse()
    {
        Span<char> buffer = stackalloc char[2];
        Millify.TryFormat(12345678901234567890m, buffer, out var written).Should().BeFalse();
        written.Should().Be(0);
    }
}
