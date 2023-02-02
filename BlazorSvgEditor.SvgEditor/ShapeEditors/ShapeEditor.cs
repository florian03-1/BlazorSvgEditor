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
        if(SvgElement.SvgEditor.EditMode != EditMode.None) return;
        SvgElement.HoverShape();
    }
    
    public void Leave()
    {
        SvgElement.UnHoverShape();
    }

    public void Select(PointerEventArgs eventArgs)
    {
        if (SvgElement.SvgEditor.EditMode == EditMode.Add) return;
        
        SvgElement.SelectShape();
        SvgElement.SvgEditor.SelectShape(SvgElement, eventArgs);
    }
    
    public void OnAnchorSelected(int anchorIndex)
    {
        SvgElement.SvgEditor.EditMode = EditMode.MoveAnchor;
        SvgElement.SvgEditor.SelectedShape = SvgElement;
        SvgElement.SvgEditor.SelectedAnchorIndex = anchorIndex;
    }
}