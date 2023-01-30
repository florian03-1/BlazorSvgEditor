using Microsoft.AspNetCore.Components;

namespace BlazorSvgEditor.SvgEditor.ShapeEditors;

public abstract class ShapeEditor<TShape> : ComponentBase where TShape : Shape
{
    [Parameter]
    public TShape SvgElement { get; set; }
    
    public ElementReference ElementReference { get; set; }
}