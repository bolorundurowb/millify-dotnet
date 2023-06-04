namespace millify.Tests;

public class MillifyOptionsTests
{
    [Fact]
    public void Constructor_DefaultValues()
    {
        // Arrange & Act
        var options = new MillifyOptions();

        // Assert
        options.Precision.Should().Be(1);
        options.Lowercase.Should().BeFalse();
        options.SpaceBeforeUnit.Should().BeFalse();
        options.Units.Should().BeNull();
    }

    [Fact]
    public void Constructor_CustomValues()
    {
        // Arrange
        var precision = 2;
        var lowercase = true;
        var spaceBeforeUnit = true;
        IEnumerable<string> units = new[] { "K", "M", "B" };

        // Act
        var options = new MillifyOptions(precision, lowercase, spaceBeforeUnit, units);

        // Assert
        options.Precision.Should().Be(precision);
        options.Lowercase.Should().Be(lowercase);
        options.SpaceBeforeUnit.Should().Be(spaceBeforeUnit);
        options.Units.Should().Equal(units);
    }

    [Fact]
    public void Constructor_InvalidPrecision()
    {
        // Arrange & Act & Assert
        FluentActions.Invoking(() => new MillifyOptions(0))
            .Should().Throw<ArgumentException>()
            .WithMessage("Invalid precision value.");
    }
}
