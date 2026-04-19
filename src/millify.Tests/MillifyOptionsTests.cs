namespace MillifyDotnet.Tests;

public class MillifyOptionsTests
{
    [Fact]
    public void MillifyOptions_WithDefaultConstructor_SetsExpectedDefaults()
    {
        var options = new MillifyOptions();

        options.Precision.Should().Be(1);
        options.Lowercase.Should().BeFalse();
        options.SpaceBeforeUnit.Should().BeFalse();
        options.ScaleBase.Should().Be(MillifyScaleBase.Decimal);
        options.TrimInsignificantZeros.Should().BeTrue();
        options.SmartPrecision.Should().BeFalse();
        options.Culture.Should().BeNull();
        options.Units.Should().Equal(string.Empty, "k", "m", "g", "t", "p", "e", "z", "y");
    }

    [Fact]
    public void MillifyOptions_WithCustomConstructorArguments_PreservesSuppliedValues()
    {
        const int precision = 2;
        const bool lowercase = true;
        const bool spaceBeforeUnit = true;
        IEnumerable<string> units = new[] { "K", "M", "B" };

        var options = new MillifyOptions(precision, lowercase, spaceBeforeUnit, units);

        options.Precision.Should().Be(precision);
        options.Lowercase.Should().Be(lowercase);
        options.SpaceBeforeUnit.Should().Be(spaceBeforeUnit);
        options.Units.Should().Equal(units);
    }

    [Fact]
    public void MillifyOptions_WithInvalidPrecision_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(0))
            .Should().Throw<ArgumentException>()
            .WithMessage("Invalid precision value.");
    }

    [Fact]
    public void MillifyOptions_WithEmptyUnitsArray_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: Array.Empty<string>()))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units must contain at least one entry.*");
    }

    [Fact]
    public void MillifyOptions_WithNullUnitEntry_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: new[] { "", "K", null! }))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units[2] must not be null.*");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsSetToEmptyArray_ThrowsArgumentException()
    {
        var options = new MillifyOptions();
        FluentActions.Invoking(() => options.Units = Array.Empty<string>())
            .Should().Throw<ArgumentException>();
    }
}
