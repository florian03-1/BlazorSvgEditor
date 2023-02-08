using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.Shapes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    //Css Class and Style Properties (for the Container)
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string CssStyle { get; set; } = string.Empty;
    
    
    [Parameter] public bool ShowDiagnosticInformation { get; set; } = false; //Show Diagnostic Information (for debugging)

    [Parameter] public (int Width, int Height) ImageSize { get; set; }
    [Parameter] public string ImageSource { get; set; } = string.Empty;  //Can be an link or also a base64 string

    public bool SnapToInteger { get; set; } = true; //Snap to integer coordinates (for performance reasons, it has to be true actually)

    
    public BoundingBox ImageBoundingBox = new();

    
    [Parameter] public bool ScaleShapes { get; set; } = true; //Scale shapes with the container

    //Must be between 0.05 and 0.5
    [Parameter] public double MinScale { get; set; } = 0.4;

    //Must be between 1 and 10
    [Parameter] public double MaxScale { get; set; } = 5;
    
    
    public List<Shape> Shapes { get; set; } = new();  //List of all shapes, is no parameter
    
    
    [Parameter] public EventCallback<ShapeChangedEventArgs> OnShapeChanged { get; set; } //Event for shape changes
    
    
    
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
    
    //SelectedShapeId is the CustomId of the selected shape (public and bindable)
    [Parameter]
#pragma warning disable BL0007
    public int SelectedShapeId
#pragma warning restore BL0007
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
    [Parameter] public EventCallback<int> SelectedShapeIdChanged { get; set; }
    
    
    
    //Func for ImageSource Loading Task
    [Parameter] public Func<Task<string>>? ImageSourceLoadingFunc { get; set; }
    [Parameter] public Func<Task<(int Width, int Height)>>? ImageSizeLoadingFunc { get; set; }
    private bool _imageSourceLoading = false;
    private bool _showLoadingSpinner => ImageSourceLoadingFunc != null && _imageSourceLoading;
    [Parameter] public RenderFragment? LoadingSpinner { get; set; }
    
    
    [Parameter] public bool EnableImageManipulations { get; set; } = true; //Use Image Manipulations (Brightness, Contrast, Saturation, Hue)
    [Parameter] public ImageManipulations ImageManipulations { get; set; } = new(); //Image Manipulations (Brightness, Contrast, Saturation, Hue)
    
    public EditMode EditMode { get; set; } = EditMode.None;  //Current edit mode
    public int? SelectedAnchorIndex { get; set; } = null; //Selected Anchor Index


    protected override Task OnParametersSetAsync()
    {
        ImageBoundingBox = new BoundingBox(ImageSize.Width, ImageSize.Height);  //Set the ImageBoundingBox to the new ImageSize
        
        return base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    { 
        //Check if MinScale is between 0.05 and 0.8
        if (MinScale < 0.05) MinScale = 0.05;
        else if (MinScale > 0.8) MinScale = 0.8;

        //Check if MaxScale is between 1 and 10
        if (MaxScale < 1) MaxScale = 1;
        else if (MaxScale > 10) MaxScale = 10;
        
        //Initialize the task for JsInvokeAsync
        moduleTask = new(async () => await JsRuntime.InvokeAsync<IJSObjectReference>("import","./_content/BlazorSvgEditor.SvgEditor/svgEditor.js"));

        await base.OnInitializedAsync();
    }
    
    
    //Css Class and Style for the Container
    private string _containerCssStyle => CssStyle;

    private string _containerCssClass {
        get
        {
            string result = CssClass + " ";
            
            if (EditMode != EditMode.AddTool) return result.Trim();
            
            return ShapeType switch
            {
                ShapeType.Polygon => result + "cursor-add-polygon",
                ShapeType.Rectangle => result + "cursor-add-rectangle",
                ShapeType.Circle => result + "cursor-add-circle",
                _ => result.Trim()
            };
        }
    }
    
    
    
    
    //Methods for component communication
    public async Task AddExistingShape(Shape shape)
    {
        Shapes.Add(shape);
        StateHasChanged();
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
        
        if (ImageSourceLoadingFunc != null) ImageSource = await ImageSourceLoadingFunc();
        if (ImageSizeLoadingFunc != null) ImageSize = await ImageSizeLoadingFunc();
        
        _imageSourceLoading = false;
        StateHasChanged();
    }
    
}