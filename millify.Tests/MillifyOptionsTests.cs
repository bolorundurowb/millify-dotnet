namespace millify.Tests;

public class MillifyOptionsTests
{
    [Fact]
    public void Constructor_DefaultValues()
    {
        var options = new MillifyOptions();

        options.Precision.Should().Be(1);
        options.Lowercase.Should().BeFalse();
        options.SpaceBeforeUnit.Should().BeFalse();
        options.Units.Should().BeNull();
    }

    [Fact]
    public void Constructor_CustomValues()
    {
        var precision = 2;
        var lowercase = true;
        var spaceBeforeUnit = true;
        IEnumerable<string> units = new[] { "K", "M", "B" };

        var options = new MillifyOptions(precision, lowercase, spaceBeforeUnit, units);

        options.Precision.Should().Be(precision);
        options.Lowercase.Should().Be(lowercase);
        options.SpaceBeforeUnit.Should().Be(spaceBeforeUnit);
        options.Units.Should().Equal(units);
    }

    [Fact]
    public void Constructor_InvalidPrecision()
    {
        FluentActions.Invoking(() => new MillifyOptions(0))
            .Should().Throw<ArgumentException>()
            .WithMessage("Invalid precision value.");
    }
}
