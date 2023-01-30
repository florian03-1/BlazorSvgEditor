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
    public string Stroke { get; set; } = string.Empty;
    public string StrokeWidth { get; set; } = string.Empty;
    
    public string StrokeLinejoin { get; set; } = "round";
    public string StrokeLinecap { get; set; } = "round";
    public string StrokeDasharray { get; set; } = string.Empty;
    public double StrokeDashoffset { get; set; }

    public abstract void SnapToInteger();
    public abstract void HandlePointerMove(PointerEventArgs eventArgs);
    public abstract void HandlePointerUp(PointerEventArgs eventArgs);
    public abstract void HandlePointerOut(PointerEventArgs eventArgs);
    public abstract void Complete();
}