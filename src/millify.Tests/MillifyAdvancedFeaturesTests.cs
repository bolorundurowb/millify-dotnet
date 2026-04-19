using System.Globalization;
using System.Numerics;

namespace millify.Tests;

public class MillifyAdvancedFeaturesTests
{
    [Fact]
    public void TrimInsignificantZeros_RemovesTrailingFractionalZeros()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: true);
        Millify.Shorten(2500, options).Should().Be("2.5K");
    }

    [Fact]
    public void TrimInsignificantZeros_CanBeDisabled()
    {
        var options = new MillifyOptions(precision: 2, trimInsignificantZeros: false);
        Millify.Shorten(2500, options).Should().Be("2.50K");
    }

    [Fact]
    public void SmartPrecision_AdjustsFractionalDigitsByMagnitude()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true);
        Millify.Shorten(9990, options).Should().Be("9.99K");
        Millify.Shorten(125_000_000, options).Should().Be("125M");
    }

    [Fact]
    public void BinaryScale_Uses1024StepsAndIecStyleSuffixCasing()
    {
        var options = new MillifyOptions(precision: 2, scaleBase: MillifyScaleBase.Binary);
        Millify.Shorten(1024, options).Should().Be("1Ki");
        Millify.Shorten(1048576, options).Should().Be("1Mi");
    }

    [Fact]
    public void BigInteger_ShortensLargeValues()
    {
        var value = BigInteger.Parse("12345678901234567890");
        var options = new MillifyOptions(precision: 2);
        Millify.Shorten(value, options).Should().Be("12.35E");
    }

    [Fact]
    public void BigInteger_PreservesFractionalScaling()
    {
        var options = new MillifyOptions(precision: 1);
        Millify.Shorten(new BigInteger(1234), options).Should().Be("1.2K");
    }

    [Fact]
    public void Culture_UsesDecimalSeparatorOnly()
    {
        var culture = new CultureInfo("fr-FR");
        var options = new MillifyOptions(precision: 1, culture: culture);
        Millify.Shorten(1234.5m, options).Should().Be("1,2K");
    }

    [Fact]
    public void Decompose_RoundTripsThroughFormatScaled()
    {
        var options = new MillifyOptions(precision: 2, smartPrecision: true, culture: new CultureInfo("de-DE"));
        var parts = Millify.Decompose(9990m, options);
        parts.ScaledValue.Should().Be(9.99m);
        parts.UnitIndex.Should().Be(1);
        Millify.FormatScaled(parts, options).Should().Be(Millify.Shorten(9990m, options));
    }

    [Fact]
    public void MillifiedNumber_ToFormattedString_MatchesShorten()
    {
        var options = new MillifyOptions(precision: 2);
        var parts = Millify.Decompose(2500m, options);
        parts.ToFormattedString(options).Should().Be(Millify.Shorten(2500m, options));
    }

    [Fact]
    public void TryFormat_WritesIntoSpan_WhenLargeEnough()
    {
        Span<char> buffer = stackalloc char[32];
        Millify.TryFormat(1234.5m, buffer, out var written).Should().BeTrue();
        written.Should().Be(4);
        buffer.Slice(0, written).ToString().Should().Be("1.2K");
    }

    [Fact]
    public void TryFormat_ReturnsFalse_WhenBufferTooSmall()
    {
        Span<char> buffer = stackalloc char[2];
        Millify.TryFormat(12345678901234567890m, buffer, out var written).Should().BeFalse();
        written.Should().Be(0);
    }

    [Fact]
    public void MillifyOptions_RejectsEmptyUnitsArray()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: Array.Empty<string>()))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units must contain at least one entry.*");
    }

    [Fact]
    public void MillifyOptions_RejectsNullUnitEntry()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: new[] { "", "K", null! }))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units[2] must not be null.*");
    }

    [Fact]
    public void MillifyOptions_UnitsSetter_RejectsInvalidAssignments()
    {
        var options = new MillifyOptions();
        FluentActions.Invoking(() => options.Units = Array.Empty<string>())
            .Should().Throw<ArgumentException>();
    }
}
