namespace MillifyDotnet;

/// <summary>
/// Represents a numeric value after magnitude scaling, before final suffix and formatting rules are applied.
/// </summary>
public readonly struct MillifiedNumber
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MillifiedNumber"/> struct.
    /// </summary>
    /// <param name="scaledValue">The signed scaled magnitude (already divided by the chosen base the required number of times).</param>
    /// <param name="unitIndex">Zero-based index into <see cref="MillifyOptions.Units"/> for the active suffix.</param>
    public MillifiedNumber(decimal scaledValue, int unitIndex)
    {
        ScaledValue = scaledValue;
        UnitIndex = unitIndex;
    }

    /// <summary>
    /// Gets the signed scaled magnitude. The sign matches the original input.
    /// </summary>
    public decimal ScaledValue { get; }

    /// <summary>
    /// Gets the zero-based index into <see cref="MillifyOptions.Units"/> for the suffix that should be shown.
    /// </summary>
    public int UnitIndex { get; }

    /// <summary>
    /// Formats this scaled value using the supplied options (precision, trimming, culture for the decimal separator, casing, spacing).
    /// </summary>
    /// <param name="options">Formatting options. When null, defaults are used.</param>
    public string ToFormattedString(MillifyOptions? options = null) =>
        Millify.FormatScaled(this, options ?? new MillifyOptions());
}
