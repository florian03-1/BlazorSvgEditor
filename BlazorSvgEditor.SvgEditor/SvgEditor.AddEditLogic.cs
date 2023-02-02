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
        Shape? newShape = null;
        switch (ShapeType)
        {
            case ShapeType.None:
                break;
            case ShapeType.Polygon:
                break;
            case ShapeType.Rectangle:
                newShape = new Rectangle(this)
                {
                    X = DetransformOffset(e).X,
                    Y = DetransformOffset(e).Y
                };
                break;
            case ShapeType.Circle:
                newShape = new Circle(this)
                {
                    Cx = DetransformOffset(e).X,
                    Cy = DetransformOffset(e).Y
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (newShape == null) return;
        
        Shapes.Add(newShape);
        SelectedShape = newShape;
        SelectedShape.SelectShape();
                
        EditMode = EditMode.Add;
    }

}