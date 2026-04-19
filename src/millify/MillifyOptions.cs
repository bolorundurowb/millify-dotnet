using System.Globalization;

namespace MillifyDotnet;

/// <summary>
/// Represents the options that can be used to configure the millify operation.
/// </summary>
public class MillifyOptions
{
    private string[] _units = Utilities.DefaultUnits;

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
    /// <exception cref="ArgumentNullException">Thrown when null is assigned.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains null entries.</exception>
    public string[] Units
    {
        get => _units;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(Units));

            Utilities.ValidateUnits(value, nameof(Units));
            _units = value;
        }
    }

    /// <summary>
    /// Gets or sets the divisor applied between each magnitude step (1000 for decimal, 1024 for binary).
    /// </summary>
    public MillifyScaleBase ScaleBase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether trailing fractional zeros are removed (for example 2.50K becomes 2.5K).
    /// </summary>
    public bool TrimInsignificantZeros { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether fractional precision is reduced for larger scaled magnitudes.
    /// When enabled, values in [100, ∞) use 0 decimals, [10, 100) use up to 1, and [1, 10) use the configured precision;
    /// values below 1 use the configured precision.
    /// </summary>
    public bool SmartPrecision { get; set; }

    /// <summary>
    /// Gets or sets the culture used for the decimal separator only. When null, the invariant separator is used.
    /// </summary>
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MillifyOptions"/> class with specified options.
    /// </summary>
    /// <param name="precision">The precision for the millified number. Default is 1.</param>
    /// <param name="lowercase">Indicates whether the units should be in lowercase. Default is false.</param>
    /// <param name="spaceBeforeUnit">Indicates whether a space should be added before the unit. Default is false.</param>
    /// <param name="units">The units to be used for the millify operation. If null, default units will be used.</param>
    /// <param name="scaleBase">The scale base (decimal 1000 or binary 1024). Default is decimal.</param>
    /// <param name="trimInsignificantZeros">Whether to trim trailing fractional zeros. Default is true.</param>
    /// <param name="smartPrecision">Whether to apply smart fractional precision by magnitude. Default is false.</param>
    /// <param name="culture">Culture for decimal separator only. Default is null (invariant).</param>
    /// <exception cref="ArgumentException">Thrown when the provided precision is less than 1, or units are invalid.</exception>
    public MillifyOptions(int precision = 1, bool lowercase = false, bool spaceBeforeUnit = false,
        IEnumerable<string>? units = null,
        MillifyScaleBase scaleBase = MillifyScaleBase.Decimal,
        bool trimInsignificantZeros = true,
        bool smartPrecision = false,
        CultureInfo? culture = null)
    {
        precision.Validate(x => x >= 1, "Invalid precision value.");

        Units = units?.ToArray() ?? (scaleBase == MillifyScaleBase.Binary
            ? Utilities.DefaultBinaryUnits
            : Utilities.DefaultUnits);

        Precision = precision;
        Lowercase = lowercase;
        SpaceBeforeUnit = spaceBeforeUnit;
        ScaleBase = scaleBase;
        TrimInsignificantZeros = trimInsignificantZeros;
        SmartPrecision = smartPrecision;
        Culture = culture;
    }
}
