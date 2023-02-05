using BlazorSvgEditor.SvgEditor.Editor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public abstract class Shape
{
    public Shape(Editor.SvgEditor svgEditor)
    {
        SvgEditor = svgEditor;
    }
    public Editor.SvgEditor SvgEditor { get; set; }
    internal abstract Type Presenter { get; }
    
    internal string HtmlId { get; set; } = Guid.NewGuid().ToString();
    public int CustomId { get; set; } = 0;
    
    public abstract ShapeType ShapeType { get; }

    internal string Fill { get; set; } = "transparent";
    internal double FillOpacity { get; set; } = 1;
    internal string Stroke { get; set; } = "#ff8c00"; //Orange
    internal string StrokeWidth { get; set; } = "2px"; 
    
    internal string StrokeLinejoin { get; set; } = "round";
    internal string StrokeLinecap { get; set; } = "round";
    internal string StrokeDasharray { get; set; } = string.Empty;
    internal double StrokeDashoffset { get; set; }

    internal ShapeState State { get; set; } = ShapeState.None;


    //Logic for visual styles - for changing selectedState use method from SvgEditor
    internal void SelectShape()
    {
        State = ShapeState.Selected;

        //Visual select logic
        StrokeWidth = "3px";
        StrokeDasharray = "5";
        StrokeDashoffset = 0;
        Fill = "#ff8c00";
        FillOpacity = 0.4;
    }
    
    internal void UnSelectShape()
    {
        State = ShapeState.None;
        
        //Visual unselect logic
        StrokeWidth = "2px";
        StrokeDasharray = string.Empty;
        Fill = "transparent";
        FillOpacity = 1;
    }
    
    internal void HoverShape()
    {
        if (State == ShapeState.Selected) return;

        State = ShapeState.Hovered;
        
        //Visual hover logic
        Fill = "#ff8c00";
        FillOpacity = 0.2;
    }
    
    internal void UnHoverShape()
    {
        if (State != ShapeState.Hovered) return;
        
        State = ShapeState.None;
        
        //Visual unhover logic
        Fill = "transparent";
        FillOpacity = 1;
    }
    
    protected abstract BoundingBox Bounds { get; }

    internal abstract void SnapToInteger();
    internal abstract void HandlePointerMove(PointerEventArgs eventArgs);
    internal abstract Task HandlePointerUp(PointerEventArgs eventArgs);
    internal abstract void HandlePointerOut(PointerEventArgs eventArgs);
    internal abstract void Complete();

    
    protected async Task FireOnShapeChangedMove() => await SvgEditor.OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeMoved(this));
    protected async Task FireOnShapeChangedEdit() => await SvgEditor.OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeEdited(this));

    public override string ToString()
    {
        return $"{GetType().Name}: {Bounds}";
    }
}

internal enum ShapeState
{
    None, 
    Selected,
    Hovered,
}