using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.ShapeEditors;

public abstract class ShapeEditor<TShape> : ComponentBase where TShape : Shape
{
    [Parameter]
    public TShape SvgElement { get; set; }
    
    public ElementReference ElementReference { get; set; }


    public void Enter()
    {
        SvgElement.Hover();
    }
    
    public void Leave()
    {
        SvgElement.Unhover();
    }

    public void Select(PointerEventArgs eventArgs)
    {
        if (SvgElement.SvgEditor.EditMode == EditMode.Add) return;
        
        SvgElement.SvgEditor.EditMode = EditMode.Move;
        SvgElement.SvgEditor.SelectedShape = SvgElement;
        SvgElement.SvgEditor.MoveStartDPoint = SvgElement.SvgEditor.DetransformOffset(eventArgs);
        SvgElement.Select();
    }
}