using BlazorSvgEditor.SvgEditor.Helper;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    //TRANSFORMATION Logic

    private double Scale = 1;
    private Coord<double> Translate;
    
    private bool IsTranslating = false;
    
    //Delta is the amount of change in the mouse wheel (+ -> zoom in, - -> zoom out)
    public void Zoom(double delta, double x, double y)
    {
        var previousScale = Scale;
        Scale = Scale * (1 - delta / 1000.0);
        
        //Translate.X += (x - Translate.X) * (1-Scale/previousScale);
        //Translate.Y += (y - Translate.Y) * (1-Scale/previousScale);
    }
    
    //x and y are the amount of change the current translation 
    public void Pan(double x, double y)
    {
        Translate.X += x;
        Translate.Y += y;
    }
    
    
    public void ResetTransformation()
    {
        var conainerRelation = BoundingBox.Width / BoundingBox.Height;
        var imageRelation = (double)ImageSize.Width / ImageSize.Height;
        
        Console.WriteLine($"conainerRelation: {conainerRelation}");
        Console.WriteLine($"imageRelation: {imageRelation}");
        
        if(imageRelation < conainerRelation) Scale = (ImageSize.Height / BoundingBox.Height) * 2;
        
        Translate = Coord<double>.Zero;
    }
}