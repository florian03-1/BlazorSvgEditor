using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private Coord<double> _lastTouchPoint = new(0, 0);
    private Coord<double> _lastZoomCenterPoint = new(0, 0);
    private double _lastTouchDistance = 0;
    
    private async Task OnTouchMove(TouchEventArgs touchEventArgs)
    {
        if (touchEventArgs.Touches.Length == 3) //Touch mit 3 Finger (Panning)
        {
            //Es wird immer um den Abstand zwischen dem letzen und dem aktuellen  Touchpunkt gepannt (bei drei Fingern wird immer der 1. genommen)
            
            var currentTouchPoint = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
            if (_lastTouchPoint != new Coord<double>(0, 0))
            {
                var delta = currentTouchPoint - _lastTouchPoint;
                Pan(delta.X, delta.Y);
            }
            _lastTouchPoint = currentTouchPoint;
        }
        else
        {
            //Falls während dem Touchvorgang auf 2 Finger gewechselt wird, muss der Ankerpunkt für die Panning-Berechnung zurückgesetzt werden
            _lastTouchPoint = new Coord<double>(0, 0); 
        }

        if (touchEventArgs.Touches.Length == 2) //Touch mit 2 Fingern (Zooming)
        {
            //Es wird sowohl gezoomt als auch gepannt, um den Mittelpunkt der beiden Finger zu halten
            
            var p1 = new Coord<double>(touchEventArgs.Touches[0].ClientX, touchEventArgs.Touches[0].ClientY);
            var p2 = new Coord<double>(touchEventArgs.Touches[1].ClientX, touchEventArgs.Touches[1].ClientY);
            var currentDistance = Coord<double>.Distance(p1, p2); //Distanz zwischen den beiden Fingern
            
            if (_lastTouchDistance != 0)
            {
                var currentTouchPoint = new Coord<double>((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                var delta = currentTouchPoint - _lastZoomCenterPoint;
                var containerCenter = new Coord<double>(_containerBoundingBox.Width / 2, _containerBoundingBox.Height / 2);
                
                var distanceDelta = currentDistance - _lastTouchDistance;
                TouchZoom(distanceDelta ,containerCenter, delta);
            }
            
            _lastZoomCenterPoint = new Coord<double>((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            _lastTouchDistance = currentDistance;
        }
        await Task.Yield();
    }
    
    private async Task OnTouchStart(TouchEventArgs touchEventArgs)
    {
        //Wird benötigt, um den Mittelpunkt des Containers für das Zooming berechnen - hier wird es nur einmal zum Beginn des Touchvorgangs berechnet
        await SetContainerBoundingBox();
    }
    
    private async Task OnTouchEnd(TouchEventArgs touchEventArgs)
    {
        _lastTouchPoint = new Coord<double>(0, 0);
        _lastZoomCenterPoint = new Coord<double>(0, 0);
        _lastTouchDistance = 0;
        await Task.Yield();
    }
}