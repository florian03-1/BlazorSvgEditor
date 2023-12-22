using BlazorSvgEditor.SvgEditor.Helper;
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

    [Parameter] public double MinScale { get; set; } = 0.4;

    [Parameter] public double MaxScale { get; set; } = 5;
    
    //Touch Settings
    
    /// <summary>
    /// Divides the touch input by 1/TouchSensitivity (higher touch sensitivity means less sensitivity)
    /// </summary>
    [Parameter] public double TouchSensitivity { get; set; } = 40;
    private int touchSensitivity => (int) (10000 / TouchSensitivity);
    
    
    public List<Shape> Shapes { get; set; } = new();  //List of all shapes, is no parameter
    
    
    [Parameter] public EventCallback<ShapeChangedEventArgs> OnShapeChanged { get; set; } //Event for shape changes
    
    [Parameter] public EventCallback<(Coord<double> translate, double scale)> TranslationChanged { get; set; }
    private async Task InvokeTranslationChanged() => await TranslationChanged.InvokeAsync((Translate, Scale));
    
    //ReadOnly
    [Parameter] public bool ReadOnly { get; set; } = false; //Is the editor read only?
    
    
    //Selected Shape (intern property) and SelectedShapeId (public property)
    private Shape? _selectedShape;
    public Shape? SelectedShape
    {
        get => _selectedShape;
        set
        {
            if (_selectedShape != value)
            {
                if(ShowDiagnosticInformation) Console.WriteLine("SelectedShape changed from " + _selectedShape?.CustomId + " to " + value?.CustomId);
                _selectedShape = value;
                SelectedShapeIdChanged.InvokeAsync(SelectedShapeId);
            }
        }
    }
    
    //SelectedShapeId is the CustomId of the selected shape (public and bindable)
    [Parameter]
    public int SelectedShapeId
    {
        get => SelectedShape?.CustomId ?? 0;
        set
        {
            if(value == SelectedShapeId) return;
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
    [Parameter] public EventCallback<int> OnShapeClicked { get; set; }



    //Func for ImageSource Loading Task
    [Parameter] public Func<Task<(string imageSource, int width, int height)>>? ImageSourceLoadingFunc { get; set; }
    [Parameter] public RenderFragment? LoadingSpinner { get; set; }

    private bool _imageSourceLoading = false;
    private bool ShowLoadingSpinner => ImageSourceLoadingFunc != null && _imageSourceLoading;
    
    
    
    //Image Manipulations
    [Parameter] public bool EnableImageManipulations { get; set; } = true; //Use Image Manipulations (Brightness, Contrast, Saturation, Hue)
    [Parameter] public ImageManipulations? ImageManipulations { get; set; } = new(); //Image Manipulations (Brightness, Contrast, Saturation, Hue)
    
    
    public EditMode EditMode { get; set; } = EditMode.None;  //Current edit mode
    public int? SelectedAnchorIndex { get; set; } = null; //Selected Anchor Index


    protected override Task OnParametersSetAsync()
    {
        ImageBoundingBox = new BoundingBox(ImageSize.Width, ImageSize.Height);  //Set the ImageBoundingBox to the new ImageSize
        
        return base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        //Initialize the task for JsInvokeAsync
        moduleTask = new(async () => await JsRuntime.InvokeAsync<IJSObjectReference>("import","./_content/BlazorSvgEditor/svgEditor.js"));

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
}