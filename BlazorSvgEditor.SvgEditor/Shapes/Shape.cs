using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public abstract class Shape
{
    public Shape(SvgEditor svgEditor)
    {
        SvgEditor = svgEditor;
    }
    
    public SvgEditor SvgEditor { get; set; }
    public abstract Type Presenter { get; }
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Fill { get; set; } = string.Empty;
    public double FillOpacity { get; set; } = 1;
    public string Stroke { get; set; } = "black";
    public string StrokeWidth { get; set; } = "2px";
    
    public string StrokeLinejoin { get; set; } = "round";
    public string StrokeLinecap { get; set; } = "round";
    public string StrokeDasharray { get; set; } = string.Empty;
    public double StrokeDashoffset { get; set; }

    public ShapeState State { get; set; } = ShapeState.None;
    public void Select()
    {
        SvgEditor.SelectedShape = this;
        State = ShapeState.Selected;
        
        Fill = "red";
        FillOpacity = 0.8;
    }
    public void Unselect()
    {
        if (State != ShapeState.Selected) return;
        
        State = ShapeState.None;
        
        Fill = "transparent";
        FillOpacity = 1;
    }
    public void Hover()
    {
        if (State == ShapeState.Selected) return;
        State = ShapeState.Hovered;
        
        Fill = "blue";
        FillOpacity = 0.3;
        
        Console.WriteLine("Hover");
    }
    public void Unhover()
    {
        if (State != ShapeState.Hovered) return;
        
        State = ShapeState.None;
        
        Fill = "transparent";
        FillOpacity = 1;
    }
    

    public abstract void SnapToInteger();
    public abstract void HandlePointerMove(PointerEventArgs eventArgs);
    public abstract void HandlePointerUp(PointerEventArgs eventArgs);
    public abstract void HandlePointerOut(PointerEventArgs eventArgs);
    public abstract void Complete();
}

public enum ShapeState
{
    None, 
    Selected,
    Hovered,
}