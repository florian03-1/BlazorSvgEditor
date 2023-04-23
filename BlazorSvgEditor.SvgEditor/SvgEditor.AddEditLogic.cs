using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;
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
        if(_imageSourceLoading) return;
        
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }

    private string? _newShapeColor = null;
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

        newShape.CustomId = Guid.NewGuid();

        if (_newShapeColor != null) newShape.Color = _newShapeColor;
        
        Shapes.Add(newShape);

        SelectedShape = newShape;
        SelectedShape.SelectShape();
                
        EditMode = EditMode.Add;
    }
    
    public async Task ShapeAddedCompleted(Shape shape)
    {
        SelectedShape = shape;
        SelectedShape.SelectShape();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(shape));
    }

}