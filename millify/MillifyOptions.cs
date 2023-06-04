namespace MillifyDotnet;

public class MillifyOptions
{
    public int Precision { get; set; }
    
    public bool Lowercase { get; set; }
    
    public bool SpaceBeforeUnit { get; set; }
    
    public List<string>? Units { get; set; }
    
    public MillifyOptions(int precision = 1, bool lowercase = false, bool spaceBeforeUnit = false,
        List<string>? units = null)
    {
        this.Units = units;
        this.Precision = precision;
        this.Lowercase = lowercase;
        this.SpaceBeforeUnit = spaceBeforeUnit;
    }
}
