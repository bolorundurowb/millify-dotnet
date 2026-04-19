using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace MillifyDotnet.Tests;

public class MillifyFeaturesTests
{
    [Fact]
    public void Shorten_WhenTrimInsignificantZerosIsTrue_RemovesTrailingFractionalZeros()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: true);
        Millify.Shorten(2500, options).Should().Be("2.5K");
    }

    [Fact]
    public void Shorten_WhenTrimInsignificantZerosIsFalse_PreservesTrailingFractionalZeros()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: false);
        Millify.Shorten(2500, options).Should().Be("2.50K");
    }

    [Fact]
    public void Shorten_WhenSmartPrecisionIsTrue_AdjustsFractionalPlacesByMagnitude()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(9990, options).Should().Be("9.99K");
        Millify.Shorten(125_000_000, options).Should().Be("125M");
    }

    [Fact]
    public void Shorten_WhenScaleBaseIsBinary_UsesIecSuffixesAndBase1024()
    {
        var options = new MillifyOptions(precision: 2, scaleBase: MillifyScaleBase.Binary);
        Millify.Shorten(1024, options).Should().Be("1Ki");
        Millify.Shorten(1048576, options).Should().Be("1Mi");
    }

    [Fact]
    public void Shorten_WhenValueIsLargeBigInteger_ScalesIntoExaRangeWithTwoDecimals()
    {
        var value = BigInteger.Parse("12345678901234567890");
        var options = new MillifyOptions(precision: 2);
        Millify.Shorten(value, options).Should().Be("12.35E");
    }

    [Fact]
    public void Shorten_WhenValueIsSmallBigInteger_PreservesFractionalThousandsStep()
    {
        var options = new MillifyOptions(precision: 1);
        Millify.Shorten(new BigInteger(1234), options).Should().Be("1.2K");
    }

    [Fact]
    public void Shorten_WhenCultureIsFrench_UsesCommaAsDecimalSeparator()
    {
        var culture = new CultureInfo("fr-FR");
        var options = new MillifyOptions(precision: 1, culture: culture);
        Millify.Shorten(1234.5m, options).Should().Be("1,2K");
    }

    [Fact]
    public void FormatScaled_WhenCultureIsGermanAndSmartPrecisionIsTrue_MatchesShortenOutput()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true, culture: new CultureInfo("de-DE"));
        var parts = Millify.Decompose(9990m, options);
        parts.ScaledValue.Should().Be(9.99m);
        parts.UnitIndex.Should().Be(1);
        Millify.FormatScaled(parts, options).Should().Be(Millify.Shorten(9990m, options));
    }

    [Fact]
    public void MillifiedNumber_ToFormattedString_WithOptions_MatchesShortenOutput()
    {
        var options = new MillifyOptions(precision: 2);
        var parts = Millify.Decompose(2500m, options);
        parts.ToFormattedString(options).Should().Be(Millify.Shorten(2500m, options));
    }

    [Fact]
    public void TryFormat_WhenBufferIsSufficient_ReturnsTrueAndWritesAbbreviation()
    {
        Span<char> buffer = stackalloc char[32];
        Millify.TryFormat(1234.5m, buffer, out var written).Should().BeTrue();
        written.Should().Be(4);
        buffer.Slice(0, written).ToString().Should().Be("1.2K");
    }

    [Fact]
    public void TryFormat_WhenBufferIsInsufficient_ReturnsFalseAndWritesNothing()
    {
        Span<char> buffer = stackalloc char[2];
        Millify.TryFormat(12345678901234567890m, buffer, out var written).Should().BeFalse();
        written.Should().Be(0);
    }

    [Fact]
    public void Decompose_OverloadsForIntegralAndFloatingPoint_MatchDecimalPath()
    {
        const long asLong = 1_234_567L;
        const double asDouble = 1_234_567d;
        var fromLong = Millify.Decompose(asLong);
        var fromDouble = Millify.Decompose(asDouble);
        var fromDecimal = Millify.Decompose(1_234_567m);
        fromLong.Should().Be(fromDecimal);
        fromDouble.Should().Be(fromDecimal);
    }

    [Fact]
    public void TryFormat_OverloadsForLongDoubleAndBigInteger_WriteSameAsShorten()
    {
        Span<char> buffer = stackalloc char[64];
        var options = new MillifyOptions(precision: 2);

        Millify.TryFormat(12_345L, buffer, out var wLong, options).Should().BeTrue();
        buffer.Slice(0, wLong).ToString().Should().Be(Millify.Shorten(12_345L, options));

        Millify.TryFormat(12_345.67d, buffer, out var wDouble, options).Should().BeTrue();
        buffer.Slice(0, wDouble).ToString().Should().Be(Millify.Shorten(12_345.67d, options));

        var bi = new BigInteger(12_345);
        Millify.TryFormat(bi, buffer, out var wBi, options).Should().BeTrue();
        buffer.Slice(0, wBi).ToString().Should().Be(Millify.Shorten(bi, options));
    }

    [Fact]
    public void MillifiedNumber_ToFormattedString_WhenOptionsIsNull_UsesDefaultFormatting()
    {
        var parts = new MillifiedNumber(1.2m, 1);
        parts.ToFormattedString(null).Should().Be(Millify.FormatScaled(parts, null));
    }

    [Fact]
    public void Decompose_WhenBigIntegerIsNegative_AppliesSignToScaledMagnitude()
    {
        var parts = Millify.Decompose(new BigInteger(-2500));
        parts.ScaledValue.Should().Be(-2.5m);
        parts.UnitIndex.Should().Be(1);
    }

    [Fact]
    public void Shorten_WhenBigIntegerExceedsDecimalRange_UsesFallbackWithoutThrowing()
    {
        var huge = BigInteger.Pow(10, 40);
        FluentActions.Invoking(() => Millify.Shorten(huge)).Should().NotThrow();
        var result = Millify.Shorten(huge);
        result.Should().NotBeNullOrEmpty();
        result.Should().EndWith("Y");
    }

    [Fact]
    public void Shorten_WhenBigIntegerIsTooLargeForDoubleFallback_ThrowsOverflowExceptionWithGuidance()
    {
        var tooLargeForDouble = BigInteger.Pow(10, 400);
        FluentActions.Invoking(() => Millify.Shorten(tooLargeForDouble))
            .Should().Throw<OverflowException>()
            .WithMessage("*representable range*");
    }

    [Fact]
    public void Shorten_WhenSmartPrecisionIsTrue_UsesSingleFractionalDigitForScaledTens()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(12_345, options).Should().Be("12.3K");
    }

    [Fact]
    public void Shorten_WhenSmartPrecisionIsTrue_UsesFullPrecisionForScaledOnes()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(5_500, options).Should().Be("5.5K");
    }

    [Fact]
    public void Shorten_WhenSmartPrecisionIsTrue_UsesFullPrecisionWhenScaledMagnitudeIsBelowOne()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(0.123m, options).Should().Be("0.12");
    }

    [Fact]
    public void Shorten_WhenBinaryScaleUsesNonIecStyleUnits_UppercasesEntireSuffix()
    {
        var units = new[] { "", "K", "M" };
        var options = new MillifyOptions(precision: 1, scaleBase: MillifyScaleBase.Binary, units: units);
        Millify.Shorten(2048, options).Should().Be("2K");
    }

    [Fact]
    public void TrimTrailingZeros_WhenDecimalSeparatorIsEmpty_ReturnsOriginalString()
    {
        var trim = typeof(Millify).GetMethod("TrimTrailingZeros",
            BindingFlags.Static | BindingFlags.NonPublic);
        trim.Should().NotBeNull();
        var result = (string)trim!.Invoke(null, ["12.50", ""])!;
        result.Should().Be("12.50");
    }
}
