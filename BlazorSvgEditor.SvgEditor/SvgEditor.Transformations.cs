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
        Scale = 1;
        Translate = Coord<double>.Zero;
        StateHasChanged();
    }
}