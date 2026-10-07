namespace MillifyDotnet.Tests;

public class MillifyOptionsTests
{
    [Fact]
    public void MillifyOptions_WithDefaultConstructor_SetsExpectedDefaults()
    {
        var options = new MillifyOptions();

        options.Precision.Must().Be(1);
        options.Lowercase.Must().BeFalse();
        options.SpaceBeforeUnit.Must().BeFalse();
        options.ScaleBase.Must().Be(MillifyScaleBase.Decimal);
        options.TrimInsignificantZeros.Must().BeTrue();
        options.SmartPrecision.Must().BeFalse();
        options.Culture.Must().BeNull();
        options.Units.Must().BeSequenceEqual([string.Empty, "k", "m", "g", "t", "p", "e", "z", "y"]);
    }

    [Fact]
    public void MillifyOptions_WithCustomConstructorArguments_PreservesSuppliedValues()
    {
        const int precision = 2;
        const bool lowercase = true;
        const bool spaceBeforeUnit = true;
        IEnumerable<string> units = ["K", "M", "B"];

        var options = new MillifyOptions(precision, lowercase, spaceBeforeUnit, units);

        options.Precision.Must().Be(precision);
        options.Lowercase.Must().Be(lowercase);
        options.SpaceBeforeUnit.Must().Be(spaceBeforeUnit);
        options.Units.Must().BeSequenceEqual(units);
    }

    [Fact]
    public void MillifyOptions_WhenPrecisionIsZero_ThrowsArgumentException()
    {
        Action act = () => new MillifyOptions(0);
        act.Throws<ArgumentException>().WithMessage("Invalid precision value.");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsArrayIsEmpty_ThrowsArgumentException()
    {
        Action act = () => new MillifyOptions(units: []);
        act.Throws<ArgumentException>().WithMessageMatching("Units must contain at least one entry.*");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsContainsNull_ThrowsArgumentException()
    {
        Action act = () => new MillifyOptions(units: ["", "K", null!]);
        act.Throws<ArgumentException>().WithMessageMatching("Units[2] must not be null.*");
    }

    [Fact]
    public void MillifyOptions_WhenUnitsPropertySetToEmptyArray_ThrowsArgumentException()
    {
        var options = new MillifyOptions();
        Action act = () => options.Units = [];
        act.Throws<ArgumentException>();
    }

    [Fact]
    public void MillifyOptions_WhenUnitsPropertySetToNull_ThrowsArgumentNullException()
    {
        var options = new MillifyOptions();
        Action act = () => options.Units = null!;
        act.Throws<ArgumentNullException>()
            .Exception.ParamName.Must().Be(nameof(MillifyOptions.Units));
    }

    [Fact]
    public void MillifyOptions_WhenScaleBaseIsBinaryAndUnitsOmitted_UsesDefaultBinarySuffixes()
    {
        var options = new MillifyOptions(scaleBase: MillifyScaleBase.Binary, units: null);
        options.Units.Must().BeSequenceEqual([string.Empty, "ki", "mi", "gi", "ti", "pi", "ei", "zi", "yi"]);
    }
}
