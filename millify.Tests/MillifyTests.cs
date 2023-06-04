namespace millify.Tests;

public class MillifyTests
{
    [Fact]
    public void Shorten_DefaultOptions()
    {
        const double number = 1234.56;
        var result = Millify.Shorten(number);
        result.Should().Be("1.2K");
    }
    
    [Fact]
    public void Shorten_Integer()
    {
        const int number = 1234567;
        var result = Millify.Shorten(number);
        result.Should().Be("1.2M");
    }
    
    [Fact]
    public void Shorten_Long()
    {
        const long number = 1234567890L;
        var result = Millify.Shorten(number);
        result.Should().Be("1.2G");
    }
    
    [Fact]
    public void Shorten_Decimal()
    {
        const decimal number = 1234.5m;
        var result = Millify.Shorten(number);
        result.Should().Be("1.2K");
    }
    
    [Fact]
    public void Shorten_Float()
    {
        const float number = 1234.5f;
        var result = Millify.Shorten(number);
        result.Should().Be("1.2K");
    }

    [Fact]
    public void Shorten_CustomOptions()
    {
        const double number = 9876543210.98765;
        var options = new MillifyOptions(precision: 3);
        var result = Millify.Shorten(number, options);
        result.Should().Be("9.877G");

        options = new MillifyOptions(precision: 2, lowercase: true);
        result = Millify.Shorten(number, options);
        result.Should().Be("9.88g");

        options = new MillifyOptions(spaceBeforeUnit: true);
        result = Millify.Shorten(number, options);
        result.Should().Be("9.9 G");

        options = new MillifyOptions(precision: 2, units: new[] {"B", "KB", "MB", "GB", "TB"});
        result = Millify.Shorten(number, options);
        result.Should().Be("9.88GB");
    }

    [Fact]
    public void Shorten_NegativeNumber()
    {
        var number = -9876.54321;
        var options = new MillifyOptions
        {
            Precision = 2,
            Lowercase = false,
            SpaceBeforeUnit = false
        };

        var result = Millify.Shorten(number, options);
        result.Should().Be("-9.88K");
    }
}
