using BlazorSvgEditor.SvgEditor.Helper;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    //TRANSFORMATION Logic

    public double Scale = 1;
    public Coord<double> Translate;
    
    private bool IsTranslating = false;
    
    //Delta is the amount of change in the mouse wheel (+ -> zoom in, - -> zoom out)
    public void Zoom(double delta, double x, double y)
    {
        var previousScale = Scale;
        var newScale = Scale * (1 - delta / 1000.0);
        
        if (newScale > MinScale && newScale < MaxScale) Scale = newScale;
        else if (newScale < MinScale) Scale = MinScale;
        else if (newScale > MaxScale) Scale = MaxScale;
        
        Translate = new (Translate.X + (x - Translate.X) * (1 - Scale / previousScale), Translate.Y + (y - Translate.Y) * (1 - Scale / previousScale));

    }
    
    //x and y are the amount of change the current translation 
    public void Pan(double x, double y)
    {
        Translate.X += x;
        Translate.Y += y;
    }


    public void ResetTransformation()
    {
        var containerRatio = (double)ContainerBoundingBox.Width / ContainerBoundingBox.Height;
        var imageRatio = (double)ImageSize.Width / ImageSize.Height;

        Translate = Coord<double>.Zero;
        
        if (containerRatio > imageRatio)
        {
            //Das Bild passt von der Breite her in den Container, aber nicht von der Höhe her
            Scale = (double)ContainerBoundingBox.Height / ImageSize.Height;
            
            var newImageWidth = Scale * ImageSize.Width;
            Translate = new (Translate.X + (ContainerBoundingBox.Width - newImageWidth) / 2, Translate.Y);
        }
        else
        {
            Scale = (double)ContainerBoundingBox.Width / ImageSize.Width;
            
            var newImageHeight = Scale * ImageSize.Height;
            Translate = new (Translate.X, Translate.Y + (ContainerBoundingBox.Height - newImageHeight) / 2);
        }
        
        StateHasChanged();
    }
    
    
    
    //Transformation Logic
    
    //Rechnet die Koordinaten des Mauszeigers in die Koordinaten des SVG-Elements um
    public Coord<double> DetransformPoint(Coord<double> point)
    {
        Coord<double> result = new();
        result.X = (point.X - Translate.X) / Scale;
        result.Y = (point.Y - Translate.Y) / Scale;
        return result;
    }
    public Coord<double> DetransformPoint(double x, double y)
    {
        return DetransformPoint(new Coord<double>(x, y));
    }
}