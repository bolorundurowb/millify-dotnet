namespace MillifyDotnet;

public static class Millify
{
    public static string Shorten(double number, MillifyOptions? options = null)
    {
        options ??= new();
    }
}
