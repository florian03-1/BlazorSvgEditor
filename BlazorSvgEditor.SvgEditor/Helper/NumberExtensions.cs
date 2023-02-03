namespace BlazorSvgEditor.SvgEditor.Helper;

public static class NumberExtensions
{
    public static double MaxAbs(double a, double b)
    {
        var aAbs = Math.Abs(a);
        var bAbs = Math.Abs(b);
        return aAbs > bAbs ? a : b;
    }

    public static int ToInt(this double d) => Convert.ToInt32(d);
    
    //Double Comparison
    public static bool IsEqual(this double a, double b, double epsilon = 0.0001)
    {
        return Math.Abs(a - b) < epsilon;
    }
}