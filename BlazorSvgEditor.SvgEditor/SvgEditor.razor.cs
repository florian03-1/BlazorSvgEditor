using System.Collections.ObjectModel;
using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    [Parameter] public (int Width, int Height) ImageSize { get; set; }
    public BoundingBox ImageBoundingBox = new();
    [Parameter] public string ImageSource { get; set; } = string.Empty;

    [Parameter] public bool SnapToInteger { get; set; } = true;

    //Must be between 0.05 and 0.5
    [Parameter] public double MinScale { get; set; } = 0.4;

    //Must be between 1 and 10
    [Parameter] public double MaxScale { get; set; } = 5;

    private ElementReference ContainerElementReference;
    private ElementReference SvgGElementReference;
    
    
    [Parameter]
    public List<Shape> Shapes { get; set; } = new();
    [Parameter]
    public EventCallback<ShapeChangedEventArgs> OnShapeChanged { get; set; }
    
    
    
    //Selected Shape (intern property) and SelectedShapeId (public property)
    private Shape? _selectedShape;
    public Shape? SelectedShape
    {
        get => _selectedShape;
        set
        {
            if (_selectedShape != value)
            {
                _selectedShape = value;
                SelectedShapeIdChanged.InvokeAsync(SelectedShapeId);
            }
        }
    }
    

    [Parameter]
    public int SelectedShapeId
    {
        get => SelectedShape?.CustomId ?? 0;
        set
        {
            if (value == 0)
            {
                SelectedShape?.UnSelectShape();
                SelectedShape = null;
                SelectedAnchorIndex = null;
            }
            else
            {
                SelectedShape?.UnSelectShape();
                SelectedShape = Shapes.FirstOrDefault(s => s.CustomId == value);
                SelectedShape?.SelectShape();
            }
        }
    }
    [Parameter]
    public EventCallback<int> SelectedShapeIdChanged { get; set; }
    
    
    
    public EditMode EditMode { get; set; } = EditMode.None;  //Current edit mode
    public int? SelectedAnchorIndex { get; set; } = null; //Selected Anchor Index
    
    
    private string _containerCssClass {
        get
        {
            if (EditMode == EditMode.AddTool)
            {
                switch (ShapeType)
                {
                    case ShapeType.Polygon:
                        return "cursor-add-polygon";
                    case ShapeType.Rectangle:
                        return "cursor-add-rectangle";
                    case ShapeType.Circle:
                        return "cursor-add-circle";
                }
            }

            return "";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        ImageSize = (700, 394);
        ImageSource =
            "https://www.bentleymotors.com/content/dam/bentley/Master/World%20of%20Bentley/Mulliner/redesign/coachbuilt/Mulliner%20Batur%201920x1080.jpg/_jcr_content/renditions/original.image_file.700.394.file/Mulliner%20Batur%201920x1080.jpg"; //700 x 394
        ImageBoundingBox =new BoundingBox(ImageSize.Width, ImageSize.Height);
        
        //Check if MinScale is between 0.05 and 0.8
        if (MinScale < 0.05) MinScale = 0.05;
        else if (MinScale > 0.8) MinScale = 0.8;

        //Check if MaxScale is between 1 and 10
        if (MaxScale < 1) MaxScale = 1;
        else if (MaxScale > 10) MaxScale = 10;
        
        //Seed Test data
        await AddTestShapes();

        //Initialize the task for JsInvokeAsync
        moduleTask = new(async () =>
            await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/BlazorSvgEditor.SvgEditor/svgEditor.js"));

        await base.OnInitializedAsync();
    }

    private async Task AddTestShapes()
    {
        Random rnd = new();

        Shapes.Add(new Circle(this) { Cy = 300, Cx = 300, R = 40, CustomId = 1});
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(Shapes.Last()));
        
        Shapes.Add(new Rectangle(this) { Y = 50, X = 400, Height = 40, Width = 60, CustomId = 3});
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(Shapes.Last()));
        
        Shapes.Add(new Polygon(this){Points = new List<Coord<double>>(){new (500,50), new (600,50), new(600,100)}, CustomId = 5});
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(Shapes.Last()));

        
        /*var poligonPoints = new List<Coord<double>>();
        for (int i = 0; i < 100; i++)
        {
            poligonPoints.Add(new (rnd.Next(100, 400), rnd.Next(50, 350)));
        }
        
        Shapes.Add(new Polygon(this){Points = poligonPoints, CustomId = 3});
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(Shapes.Last()));*/
        
        /*var poligonPoints2 = new List<Coord<double>>();
        for (int i = 0; i < 15; i++)
        {
            poligonPoints2.Add(new (rnd.Next(500, 650), rnd.Next(50, 350)));
        }

        Shapes.Add(new Polygon(this){Points = poligonPoints2,CustomId =5});
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(Shapes.Last()));*/

    }
    
    
    
    //Methods for component communication
    public async Task AddExistingShape(Shape shape)
    {
        Shapes.Add(shape);
        await OnShapeChanged.InvokeAsync(ShapeChangedEventArgs.ShapeAdded(shape));
    }
    
    public void AddNewShape(ShapeType shapeType)
    {
        EditMode = EditMode.AddTool;
        ShapeType = shapeType;
        
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
    }
    
    public void ClearShapes()
    {
        Shapes.Clear();
    }
    
    public async Task ResetTransform()
    {
        await SetContainerAndSvgBoundingBox();
        ResetTransformation();
    } 
    
}



public class ShapeChangedEventArgs : EventArgs
{
    public ShapeChangeType ChangeType { get; set; }
    public Shape Shape { get; private set; } = null!;
    
    private int _shapeId;
    public int ShapeId => Shape?.CustomId ?? _shapeId;
    
    public static ShapeChangedEventArgs ShapeMoved(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Move,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeEdited(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Edit,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeAdded(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Add,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeDeleted(int deletedShapeId)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Delete,
            Shape = null!,
            _shapeId = deletedShapeId
        };
    }
}
public enum ShapeChangeType
{
    Move,
    Edit,
    Add,
    Delete,
    Other
}
