using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    
    public Coord<double> lastTouchPoint = new(0, 0);
    private async Task OnTouchMove(TouchEventArgs touchEventArgs)
    {
        if (lastTouchPoint != new Coord<double>(0, 0))
        {
            var currentTouchPoint = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
            var delta = currentTouchPoint - lastTouchPoint;
            Console.WriteLine("Delta: " + delta);
            Pan(delta.X, delta.Y);
        }
        lastTouchPoint = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
    }
    
    private async Task OnTouchEnd(TouchEventArgs touchEventArgs)
    {
        lastTouchPoint = new Coord<double>(0, 0);
    }
}