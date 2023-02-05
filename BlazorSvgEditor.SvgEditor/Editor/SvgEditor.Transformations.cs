using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Editor;

public partial class SvgEditor
{
    //TRANSFORMATION Logic

    private double Scale = 1;
    private Coord<double> Translate;
    
    private bool IsTranslating = false;
    
    internal Coord<double> MoveStartDPoint;

    //Delta is the amount of change in the mouse wheel (+ -> zoom in, - -> zoom out)
    private void Zoom(double delta, double x, double y)
    {
        var previousScale = Scale;
        var newScale = Scale * (1 - delta / 1000.0);
        
        if (newScale > MinScale && newScale < MaxScale) Scale = newScale;
        else if (newScale < MinScale) Scale = MinScale;
        else if (newScale > MaxScale) Scale = MaxScale;
        
        Translate = new (Translate.X + (x - Translate.X) * (1 - Scale / previousScale), Translate.Y + (y - Translate.Y) * (1 - Scale / previousScale));

    }
    
    //x and y are the amount of change the current translation 
    private void Pan(double x, double y)
    {
        Translate.X += x;
        Translate.Y += y;
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
    
    
    
    //Transformation Logic
    
    //Rechnet die Koordinaten des Mauszeigers in die Koordinaten des SVG-Elements um
    internal Coord<double> DetransformPoint(Coord<double> point)
    {
        Coord<double> result = new();
        result.X = (point.X - Translate.X) / Scale;
        result.Y = (point.Y - Translate.Y) / Scale;
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