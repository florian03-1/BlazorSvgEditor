using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    
    private Coord<double> lastTouchPoint = new(0, 0);
    private double lastDistance = 0;
    private async Task OnTouchMove(TouchEventArgs touchEventArgs)
    {
        if (touchEventArgs.Touches.Length == 1)
        {
            if (lastTouchPoint != new Coord<double>(0, 0))
            {
                var currentTouchPoint = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
                var delta = currentTouchPoint - lastTouchPoint;
                Pan(delta.X, delta.Y);
            }
            lastTouchPoint = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
        }

        if (touchEventArgs.Touches.Length == 2)
        {
            var p1 = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
            var p2 = new Coord<double>(touchEventArgs.Touches[1].ClientX, touchEventArgs.Touches[1].ClientY);
            var currentDistance = Coord<double>.Distance(p1, p2);
            
            if (lastDistance != 0)
            {
                var containerCenter = new Coord<double>(_containerBoundingBox.Width / 2, _containerBoundingBox.Height / 2);
                
                //Calculate the zoom factor
                var zoomFactor = currentDistance / lastDistance;
                
                Zoom(zoomFactor / 5, containerCenter.X, containerCenter.Y);
            }
            
            lastDistance = currentDistance;
        }
    }
    
    private async Task OnTouchStart(TouchEventArgs touchEventArgs)
    {
        await SetContainerBoundingBox();
    }
    
    private async Task OnTouchEnd(TouchEventArgs touchEventArgs)
    {
        lastTouchPoint = new Coord<double>(0, 0);
        lastDistance = 0;
    }
}