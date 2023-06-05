using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.ShapeEditors;

public abstract class ShapeEditor<TShape> : ComponentBase where TShape : Shape
{
    [Parameter, EditorRequired]
    public TShape SvgElement { get; set; } = null!; //Wird zwingend im SvgEditor bei der Initialisierung gesetzt
    
    protected ElementReference ElementReference { get; set; }


    protected void Enter(PointerEventArgs e)
    {
        if(e.PointerType == "touch") return; //Touch events are handled seperately
        
        if(SvgElement.SvgEditor.EditMode != EditMode.None) return;
        SvgElement.HoverShape();
    }
    
    protected void Leave(PointerEventArgs e)
    {
        if(e.PointerType == "touch") return; //Touch events are handled seperately
        
        SvgElement.UnHoverShape();
    }

    protected void Select(PointerEventArgs eventArgs)
    {
        if(eventArgs.PointerType == "touch") return; //Touch events are handled seperately

        if (SvgElement.SvgEditor.EditMode == EditMode.Add) return;
        if (SvgElement.SvgEditor.ReadOnly) return; //Beim ReadOnly Modus kann man keine Shapes auswählen
        
        SvgElement.SelectShape();
        SvgElement.SvgEditor.SelectShape(SvgElement, eventArgs);
    }
    
    protected void OnAnchorSelected(int anchorIndex)
    {
        SvgElement.SvgEditor.EditMode = EditMode.MoveAnchor;
        SvgElement.SvgEditor.SelectedShape = SvgElement;
        SvgElement.SvgEditor.SelectedAnchorIndex = anchorIndex;
    }
}