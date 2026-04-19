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
        IEnumerable<string> units = ["K", "M", "B"];

        var options = new MillifyOptions(precision, lowercase, spaceBeforeUnit, units);

        options.Precision.Should().Be(precision);
        options.Lowercase.Should().Be(lowercase);
        options.SpaceBeforeUnit.Should().Be(spaceBeforeUnit);
        options.Units.Should().Equal(units);
    }

    [Fact]
    public void MillifyOptions_WhenPrecisionIsZero_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(0))
            .Should().Throw<ArgumentException>()
            .WithMessage("Invalid precision value.");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsArrayIsEmpty_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: []))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units must contain at least one entry.*");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsContainsNull_ThrowsArgumentException()
    {
        FluentActions.Invoking(() => new MillifyOptions(units: ["", "K", null!]))
            .Should().Throw<ArgumentException>()
            .WithMessage("Units[2] must not be null.*");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsPropertySetToEmptyArray_ThrowsArgumentException()
    {
        var options = new MillifyOptions();
        FluentActions.Invoking(() => options.Units = [])
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MillifyOptions_WhenUnitsPropertySetToNull_ThrowsArgumentNullException()
    {
        var options = new MillifyOptions();
        FluentActions.Invoking(() => options.Units = null!)
            .Should().Throw<ArgumentNullException>()
            .WithParameterName(nameof(MillifyOptions.Units));
    }

    [Fact]
    public void MillifyOptions_WhenScaleBaseIsBinaryAndUnitsOmitted_UsesDefaultBinarySuffixes()
    {
        var options = new MillifyOptions(scaleBase: MillifyScaleBase.Binary, units: null);
        options.Units.Should().Equal(string.Empty, "ki", "mi", "gi", "ti", "pi", "ei", "zi", "yi");
    }
}
