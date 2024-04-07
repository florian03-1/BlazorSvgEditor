using BlazorSvgEditor.SvgEditor.Helper;
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
    
    public void SetEditModeToNone()
    {
        EditMode = EditMode.None;
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
        await ResetTransformation();
    }


    public async Task ZoomToShape(int shapeId, double marginPercentage)
    {
        Shape? shape = Shapes.FirstOrDefault(s => s.CustomId == shapeId);
        if (shape != null)
        {
            await SetContainerBoundingBox();
            
            var shapeWidth = shape.Bounds.Right - shape.Bounds.Left;
            var shapeHeight = shape.Bounds.Bottom - shape.Bounds.Top;
            var marginPixels = Math.Max(shapeWidth, shapeHeight) * marginPercentage;

            //Die BoundingBox des elements ist im Verhältnis breiter als die des Containers
            var shapeBoundingBoxWithMargin = new BoundingBox
            {
                Left = shape.Bounds.Left - marginPixels,
                Right = shape.Bounds.Right + marginPixels,
                Top = shape.Bounds.Top - marginPixels,
                Bottom = shape.Bounds.Bottom + marginPixels
            };
                
            ZoomToShape(shapeBoundingBoxWithMargin);
        }
    }
    
    public async Task ZoomToShape(int shapeId, int marginPixels)
    {
        Shape? shape = Shapes.FirstOrDefault(s => s.CustomId == shapeId);
        if (shape != null)
        {
            await SetContainerBoundingBox();
            
            var shapeWidth = shape.Bounds.Right - shape.Bounds.Left;
            var shapeHeight = shape.Bounds.Bottom - shape.Bounds.Top;
            bool isShapeWiderThanContainer = (shapeWidth / shapeHeight) > (_containerBoundingBox.Width / _containerBoundingBox.Height);
            
            var scaleForCalculatingMargin = isShapeWiderThanContainer ? (double)_containerBoundingBox.Width / shapeWidth : (double)_containerBoundingBox.Height / shapeHeight;

            //Die BoundingBox des elements ist im Verhältnis breiter als die des Containers
            var shapeBoundingBoxWithMargin = new BoundingBox
            {
                Left = shape.Bounds.Left - marginPixels / scaleForCalculatingMargin,
                Right = shape.Bounds.Right + marginPixels / scaleForCalculatingMargin,
                Top = shape.Bounds.Top - marginPixels / scaleForCalculatingMargin,
                Bottom = shape.Bounds.Bottom + marginPixels / scaleForCalculatingMargin
            };
                
            ZoomToShape(shapeBoundingBoxWithMargin);
        }
    }
    
    public async Task ZoomToShape(int shapeId)
    {
        await ZoomToShape(shapeId, 0.05);
    }

    
    //Use this method to set the translation to a specific value -> e.g. to syncronize the translation of two SvgEditors
    public void SetTranslateAndScale(Coord<double>? newTranslate = null, double? newScale = null)
    {
        if(newTranslate != null) Translate = newTranslate.Value;
        if (newScale != null) Scale = newScale.Value;
        StateHasChanged();
    }
    public (Coord<double> translation, double scale) GetTranslateAndScale() => (Translate, Scale);

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
        await OnImageLoaded.InvokeAsync();
        StateHasChanged();
    }
    
    public void Refresh() => StateHasChanged();
}