using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private ShapeType ShapeType { get; set; } = ShapeType.None; 

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

    private async Task AddToolPointerDown(PointerEventArgs e)
    {
        Shape? newShape = null;
        switch (ShapeType)
        {
            case ShapeType.None:
                return;
            
            case ShapeType.Polygon:
                newShape = new Polygon(this)
                {
                    Points = new List<Coord<double>>()
                    {
                        new(DetransformOffset(e))
                    }
                };
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

        var newShapeId = -1;
        if (Shapes.Count > 0) newShapeId = Math.Min(Shapes.Min(x => x.CustomId) - 1, newShapeId);

        newShape.CustomId = newShapeId;
        Shapes.Add(newShape);
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(newShape));

        SelectedShape = newShape;
        SelectedShape.SelectShape();
                
        EditMode = EditMode.Add;
    }

}