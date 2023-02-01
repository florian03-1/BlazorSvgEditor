namespace BlazorSvgEditor.SvgEditor.Helper;

public static class NumberExtensions
{
    public static double MaxAbs(double a, double b)
    {
        var aAbs = Math.Abs(a);
        var bAbs = Math.Abs(b);
        return aAbs > bAbs ? a : b;
    }
}