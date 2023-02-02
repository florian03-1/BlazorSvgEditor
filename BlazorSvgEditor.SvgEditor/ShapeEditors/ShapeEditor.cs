using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.ShapeEditors;

public abstract class ShapeEditor<TShape> : ComponentBase where TShape : Shape
{
    [Parameter, EditorRequired]
    public TShape SvgElement { get; set; } = null!; //Wird zwingend im SvgEditor bei der Initialisierung gesetzt
    
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
        
        SvgElement.Select(eventArgs);
    }
    
    public void OnAnchorSelected(int anchorIndex)
    {
        SvgElement.SvgEditor.EditMode = EditMode.MoveAnchor;
        SvgElement.SvgEditor.SelectedShape = SvgElement;
        SvgElement.SvgEditor.SelectedAnchorIndex = anchorIndex;
    }
}