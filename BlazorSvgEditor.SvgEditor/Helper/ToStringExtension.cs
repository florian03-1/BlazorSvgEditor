using System.Globalization;

namespace BlazorSvgEditor.SvgEditor.Helper;

public static class ToStringExtension
{
    public static string ToInvString(this double value) => value.ToString(CultureInfo.InvariantCulture);
}