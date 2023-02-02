using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    public void SelectShape(Shape shape, PointerEventArgs eventArgs)
    {
        SelectedShape?.UnSelectShape();
        
        SelectedShape = shape;      //Das neue Shape wird ausgewählt
        SelectedShape.SelectShape(); //Das neue Shape wird ausgewählt

        EditMode = EditMode.Move;
        MoveStartDPoint = DetransformOffset(eventArgs);
        StateHasChanged();
    }
    
    public void AddElement(ShapeType shapeType)
    {
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }

}