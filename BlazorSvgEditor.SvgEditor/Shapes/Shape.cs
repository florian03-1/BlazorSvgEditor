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

    public string Fill { get; set; } = "transparent";
    public double FillOpacity { get; set; } = 1;
    public string Stroke { get; set; } = "#ff8c00"; //Orange
    public string StrokeWidth { get; set; } = "2px"; 
    
    public string StrokeLinejoin { get; set; } = "round";
    public string StrokeLinecap { get; set; } = "round";
    public string StrokeDasharray { get; set; } = string.Empty;
    public double StrokeDashoffset { get; set; }

    public ShapeState State { get; set; } = ShapeState.None;


    //Logic for visual styles - for changing selectedState use method from SvgEditor
    public void SelectShape()
    {
        State = ShapeState.Selected;

        //Visual select logic
        StrokeWidth = "3px";
        StrokeDasharray = "5";
        StrokeDashoffset = 0;
        Fill = "#ff8c00";
        FillOpacity = 0.4;
    }
    
    public void UnSelectShape()
    {
        State = ShapeState.None;
        
        //Visual unselect logic
        StrokeWidth = "2px";
        StrokeDasharray = string.Empty;
        Fill = "transparent";
        FillOpacity = 1;
    }
    
    public void HoverShape()
    {
        if (State == ShapeState.Selected) return;

        State = ShapeState.Hovered;
        
        //Visual hover logic
        Fill = "#ff8c00";
        FillOpacity = 0.2;
    }
    
    public void UnHoverShape()
    {
        if (State != ShapeState.Hovered) return;
        
        State = ShapeState.None;
        
        //Visual unhover logic
        Fill = "transparent";
        FillOpacity = 1;
    }
    
    public abstract ContainerBox Bounds { get; }

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