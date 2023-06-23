using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;

namespace BlazorSvgEditor.WasmTest.Pages;

using BlazorSvgEditor.SvgEditor;

public partial class Preview
{
    private SvgEditor? svgEditor;
    private int SelectedShapeId { get; set; }
    private int ClickedShapeId { get; set; }    
    private bool ReadOnly { get; set; } = false; //Is the editor read only?

    
    private List<Shape> Shapes = new();
    string status = "--Status--";

    private ImageManipulations ImageManipulations = new();

    private string imageUrl = "example01.png";
    private string delayString = "1000";
    
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (svgEditor == null) return;
            await svgEditor.ReloadImage();
            await svgEditor.AddExistingShape(new Circle(svgEditor){CustomId = 1, Cx = 400, Cy = 300 , R = 100});
            await svgEditor.AddExistingShape(new Rectangle(svgEditor){CustomId = 2, X = 700, Y = 400 , Width = 100, Height = 50});
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    

    private void AddShape(ShapeType shapeType, string? color = null) => svgEditor?.AddNewShape(shapeType, color);
    private void ShapeSelected(int shapeId) => SelectedShapeId = shapeId;
    private void ResetTransform() => svgEditor?.ResetTransform();
    private void ClearAll() => svgEditor?.ClearShapes();
    

    private async Task ReloadEditorImage()
    {
        if (svgEditor == null) return;
        await svgEditor.ReloadImage();
    }

    private async Task<(string imageSource, int width, int height)> GetImageSource()
    {
        ImageManipulations = new();
        await Task.Delay(int.Parse(delayString));
        return (imageUrl, 1000, 750);
    }

    private void EditorShapeChanged(ShapeChangedEventArgs e)
    {
        if (e.ChangeType == ShapeChangeType.Add && e.Shape?.CustomId <= 0) //Wenn das Shape neu ist und es noch keine ID hat...
        {
            //Get new id
            var newId = Shapes.Any() ? Shapes.Max(x => x.CustomId) + 1 : 1;
            e.Shape.CustomId = newId;
        }


        switch (e.ChangeType)
        {
            case ShapeChangeType.Add:
                status = $"{e.Shape.ShapeType} (Id: {e.Shape.CustomId}) was added";
                break;
            case ShapeChangeType.Edit:
                status = $"{e.Shape.ShapeType} (Id: {e.Shape.CustomId}) was edited";
                break;
            case ShapeChangeType.Delete:
                status = $"Shape was deleted";
                break;
            case ShapeChangeType.ClearAll:
                status = "All shapes were deleted";
                break;
            case ShapeChangeType.Move:
                status = $"{e.Shape.ShapeType} (Id: {e.Shape.CustomId}) was moved";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (e.ChangeType == ShapeChangeType.Delete)
        {
            Shapes.Remove(Shapes.First(x => x.CustomId == e.ShapeId));
            return;
        }

        if (e.ChangeType == ShapeChangeType.ClearAll)
        {
            Shapes.Clear();
            return;
        }
        

        if (Shapes.Any(x => x.CustomId == e.Shape?.CustomId))
        {
            //Remove old shape
            Shapes.Remove(Shapes.First(x => x.CustomId == e.Shape?.CustomId));
        }

        Shapes.Add(e.Shape);

        Shapes = Shapes.OrderBy(x => x.CustomId).ToList();
    }

    private void DeleteShape()
    {
        svgEditor?.RemoveSelectedShape();
    }

    void ClickShape(int CustomId)
    {
        ClickedShapeId = CustomId;
    }
}