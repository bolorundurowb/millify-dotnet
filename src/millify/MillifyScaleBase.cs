namespace MillifyDotnet;

/// <summary>
/// Defines the divisor applied between each magnitude step when shortening a value.
/// </summary>
public enum MillifyScaleBase
{
    /// <summary>
    /// Use decimal (SI-style) steps of 1000 between units.
    /// </summary>
    Decimal = 1000,

    /// <summary>
    /// Use binary (IEC-style) steps of 1024 between units (for example byte counts).
    /// </summary>
    Binary = 1024
}
