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
    
    private void AddElement(ShapeType shapeType)
    {
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }

    public void AddToolPointerDown(PointerEventArgs e)
    {
        switch (ShapeType)
        {
            case ShapeType.None:
                break;
            case ShapeType.Polygon:
                break;
            case ShapeType.Rectangle:
                break;
            case ShapeType.Circle:
                var circle = new Circle(this);
        
                circle.Cx = DetransformOffset(e).X;
                circle.Cy = DetransformOffset(e).Y;

                Shapes.Add(circle);
                SelectedShape = circle;
                SelectedShape.SelectShape();
                
                EditMode = EditMode.Add;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}