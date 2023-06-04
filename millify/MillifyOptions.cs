namespace MillifyDotnet;

public class MillifyOptions
{
    public int Precision { get; set; }
    
    public bool Lowercase { get; set; }
    
    public bool SpaceBeforeUnit { get; set; }
    
    public string[]? Units { get; set; }
    
    public MillifyOptions(int precision = 1, bool lowercase = false, bool spaceBeforeUnit = false,
        IEnumerable<string>? units = null)
    {
        precision.Validate(x => x >= 1, "Invalid precision value.");
        
        this.Units = units?.ToArray();
        this.Precision = precision;
        this.Lowercase = lowercase;
        this.SpaceBeforeUnit = spaceBeforeUnit;
    }
}
