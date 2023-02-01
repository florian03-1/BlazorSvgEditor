using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    [Parameter] public (int Width, int Height) ImageSize { get; set; }
    public ContainerBox ImageBoundingBox = new();
    [Parameter] public string ImageSource { get; set; } = string.Empty;

    [Parameter] public bool SnapToInteger { get; set; } = true;

    //Must be between 0.05 and 0.5
    [Parameter] public double MinScale { get; set; } = 0.4;

    //Must be between 1 and 10
    [Parameter] public double MaxScale { get; set; } = 5;

    private ElementReference ContainerElementReference;
    private ElementReference SvgGElementReference;
    
    public List<Shape> Shapes { get; set; } = new();
    public Shape? SelectedShape { get; set; }

    public EditMode EditMode { get; set; } = EditMode.None;
    public int? SelectedAnchorIndex { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //await GetBoundingBox(SvgElementReference);
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        ImageSize = (700, 394);
        ImageSource =
            "https://www.bentleymotors.com/content/dam/bentley/Master/World%20of%20Bentley/Mulliner/redesign/coachbuilt/Mulliner%20Batur%201920x1080.jpg/_jcr_content/renditions/original.image_file.700.394.file/Mulliner%20Batur%201920x1080.jpg"; //700 x 394
        ImageBoundingBox =new ContainerBox(0,0,ImageSize.Width, ImageSize.Height);
        
        //Check if MinScale is between 0.05 and 0.8
        if (MinScale < 0.05) MinScale = 0.05;
        else if (MinScale > 0.8) MinScale = 0.8;

        //Check if MaxScale is between 1 and 10
        if (MaxScale < 1) MaxScale = 1;
        else if (MaxScale > 10) MaxScale = 10;

        Shapes.Add(new Circle(this) { Cy = 300, Cx = 300, R = 40 });
        Shapes.Add(new Circle(this) { Cy = 300, Cx = 400, R = 40 });


        //Initialize the task for JsInvokeAsync
        moduleTask = new(async () =>
            await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/BlazorSvgEditor.SvgEditor/svgEditor.js"));

        await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        SetContainerAndSvgBoundingBox();
        base.OnAfterRender(firstRender);
    }

    public void SelectShape(Shape shape, PointerEventArgs eventArgs)
    {
        SelectedShape?.Unselect();  //Wenn ein Shape ausgewählt ist, dann wird es abgewählt
        SelectedShape = shape;      //Das neue Shape wird ausgewählt

        EditMode = EditMode.Move;
        MoveStartDPoint = DetransformOffset(eventArgs);
    }
   
}