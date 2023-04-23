using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
      //Methods for component communication
    public async Task AddExistingShape(Shape shape)
    {
        Shapes.Add(shape);
        StateHasChanged();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(shape));
    }
    
    public void AddNewShape(ShapeType shapeType, string? color = null)
    {
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
        _newShapeColor = color;
        
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }
    
    public async Task RemoveSelectedShape()
    {
        if (SelectedShape != null)
        {
            int deletedShapeId = SelectedShape.CustomId;
            Shapes.Remove(SelectedShape);
            SelectedShape = null;
            SelectedAnchorIndex = null;
            
            await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeDeleted(deletedShapeId));
        }
        else
        {
            if (ShowDiagnosticInformation) Console.WriteLine("No shape selected - so nothing to delete");
        }
    }
    
    public async Task RemoveShape(int shapeId)
    {
        Shape? shape = Shapes.FirstOrDefault(s => s.CustomId == shapeId);
        if (shape != null)
        {
            Shapes.Remove(shape);
            await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeDeleted(shapeId));
        }
        else
        {
            if(ShowDiagnosticInformation) Console.WriteLine("Shape with id " + shapeId + " not found - so nothing to delete");
        }
    }
    
    public async Task ClearShapes()
    {
        Shapes.Clear();
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapesCleared());
    }
    
    public async Task ResetTransform()
    {
        await SetContainerBoundingBox();
        ResetTransformation();
    }

    public async Task ReloadImage()
    {
        _imageSourceLoading = true;
        StateHasChanged();

        (string imageSource, int width, int height) result;
        if (ImageSourceLoadingFunc != null)
        {
            result = await ImageSourceLoadingFunc();
            ImageSize = (result.width, result.height);
            ImageSource = result.imageSource;
        }
        
        _imageSourceLoading = false;
        StateHasChanged();
    }

    public async Task DuplicateSelectedShape()
    {
        if (SelectedShape != null)
        {
            int deletedShapeId = SelectedShape.CustomId;
            Shape? shape = Shapes.FirstOrDefault(s => s.CustomId == deletedShapeId);

            if (shape != null)
            {
                var newShape = shape.Clone();
                if (Shapes.Count > 0) newShape.CustomId = Math.Min(Enumerable.Min<Shape>(Shapes, x => x.CustomId) - 1, newShape.CustomId);

                Shapes.Add(newShape);
                await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(newShape));
            }
        }
        else
        {
            if (ShowDiagnosticInformation) Console.WriteLine("No shape selected - so nothing to duplicate");
        }
    }

    public void Refresh() => StateHasChanged();
}