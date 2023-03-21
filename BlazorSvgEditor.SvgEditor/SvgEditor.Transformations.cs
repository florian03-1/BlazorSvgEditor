using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    //TRANSFORMATION Logic

    public double Scale = 1;
    private Coord<double> Translate;
    
    private bool IsTranslating = false;
    
    internal Coord<double> MoveStartDPoint;

    //Delta is the amount of change in the mouse wheel (+ -> zoom in, - -> zoom out)
    private void Zoom(double delta, double x, double y)
    {
        var previousScale = Scale;
        var newScale = Scale * (1 - delta / 1000.0);

        if (newScale > MinScale && newScale < MaxScale) Scale = newScale.Round(3);
        else if (newScale < MinScale) Scale = MinScale;
        else if (newScale > MaxScale) Scale = MaxScale;
        
        Translate = new (Translate.X + (x - Translate.X) * (1 - Scale / previousScale), Translate.Y + (y - Translate.Y) * (1 - Scale / previousScale));
        Translate = new (Translate.X.Round(3), Translate.Y.Round(3));
    }

    private void TouchZoom(double distanceDelta, Coord<double> containerCenter, Coord<double> delta)
    {
        //DistanceDelta is the amount of change in the distance between the two fingers (+ -> zoom in, - -> zoom out)
        var distanceDeltaFactor = Scale; //Damit das Skalieren zu jeder Zeit gleichmäßig ist, wird die Distanz mit dem aktuellen Scale multipliziert
        
        if (Scale < MinScale && distanceDelta < 0) distanceDeltaFactor = 0; //Wenn Scale kleiner als MinScale ist, darf nicht herausgezoomt werden
        else if (Scale > MaxScale && distanceDelta > 0) distanceDeltaFactor = 0; //Wenn Scale größer als MaxScale ist, darf nicht hereingezoomt werden
        
        var newScale = Scale + (distanceDelta * distanceDeltaFactor) / touchSensitivity;
        
        var previousScale = Scale;
        Scale = newScale;
        
        //Set the translation, that the center of the container stays in the center of the image and pan it by the delta
        Translate = new (Translate.X + (containerCenter.X - Translate.X) * (1 - Scale / previousScale) + delta.X, Translate.Y + (containerCenter.Y - Translate.Y) * (1 - Scale / previousScale) + delta.Y);
        Translate = new (Translate.X.Round(3), Translate.Y.Round(3));
    }

    //x and y are the amount of change the current translation 
    private void Pan(double x, double y)
    {
        Translate.X = (Translate.X + x).Round(3);
        Translate.Y = (Translate.Y + y).Round(3);
    }

    

    private void ResetTransformation()
    {
        var containerRatio = (double)_containerBoundingBox.Width / _containerBoundingBox.Height;
        var imageRatio = (double)ImageSize.Width / ImageSize.Height;

        Translate = Coord<double>.Zero;
        
        if (containerRatio > imageRatio)
        {
            //Das Bild passt von der Breite her in den Container, aber nicht von der Höhe her
            Scale = (double)_containerBoundingBox.Height / ImageSize.Height;
            
            var newImageWidth = Scale * ImageSize.Width;
            Translate = new (Translate.X + (_containerBoundingBox.Width - newImageWidth) / 2, Translate.Y);
        }
        else
        {
            Scale = (double)_containerBoundingBox.Width / ImageSize.Width;
            
            var newImageHeight = Scale * ImageSize.Height;
            Translate = new (Translate.X, Translate.Y + (_containerBoundingBox.Height - newImageHeight) / 2);
        }
        
        StateHasChanged();
    }
    
    
    internal double GetScaledValue(double value, int decimals = 1) => (value *(1/ Scale )).Round(decimals);
    
    
    //Transformation Logic
    
    //Rechnet die Koordinaten des Mauszeigers in die Koordinaten des SVG-Elements um
    internal Coord<double> DetransformPoint(Coord<double> point)
    {
        Coord<double> result = new()
        {
            X = (point.X - Translate.X) / Scale,
            Y = (point.Y - Translate.Y) / Scale
        };
        return result;
    }
    internal Coord<double> DetransformPoint(double x, double y)
    {
        return DetransformPoint(new Coord<double>(x, y));
    }
    internal Coord<double> DetransformOffset(PointerEventArgs pointerEventArgs)
    {
        return DetransformPoint(new Coord<double>(pointerEventArgs.OffsetX, pointerEventArgs.OffsetY));
    }
}