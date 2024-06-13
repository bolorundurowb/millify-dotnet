namespace MillifyDotnet;

/// <summary>
/// Represents the options that can be used to configure the millify operation.
/// </summary>
public class MillifyOptions
{
    /// <summary>
    /// Gets or sets the precision for the millified number. The precision determines the number of decimal places.
    /// Must be greater than or equal to 1.
    /// </summary>
    public int Precision { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the units should be in lowercase.
    /// </summary>
    public bool Lowercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a space should be added before the unit.
    /// </summary>
    public bool SpaceBeforeUnit { get; set; }

    /// <summary>
    /// Gets or sets the units to be used for the millify operation. If no units are provided,
    /// the default units will be used.
    /// </summary>
    public string[] Units { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MillifyOptions"/> class with specified options.
    /// </summary>
    /// <param name="precision">The precision for the millified number. Default is 1.</param>
    /// <param name="lowercase">Indicates whether the units should be in lowercase. Default is false.</param>
    /// <param name="spaceBeforeUnit">Indicates whether a space should be added before the unit. Default is false.</param>
    /// <param name="units">The units to be used for the millify operation. If null, default units will be used.</param>
    /// <exception cref="ArgumentException">Thrown when the provided precision is less than 1.</exception>
    public MillifyOptions(int precision = 1, bool lowercase = false, bool spaceBeforeUnit = false,
        IEnumerable<string>? units = null)
    {
        precision.Validate(x => x >= 1, "Invalid precision value.");

        Units = units?.ToArray() ?? Utilities.DefaultUnits;
        Precision = precision;
        Lowercase = lowercase;
        SpaceBeforeUnit = spaceBeforeUnit;
    }
}
