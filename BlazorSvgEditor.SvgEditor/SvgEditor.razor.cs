using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    [Parameter] 
    public (int Width, int Height) ImageSize { get; set; } = (700, 394);
    
    [Parameter]
    public string ImageSource { get; set; } = "https://www.bentleymotors.com/content/dam/bentley/Master/World%20of%20Bentley/Mulliner/redesign/coachbuilt/Mulliner%20Batur%201920x1080.jpg/_jcr_content/renditions/original.image_file.700.394.file/Mulliner%20Batur%201920x1080.jpg";//700 x 394

    private ElementReference SvgElementReference;
    
    bool IsMooving { get; set; } = false;
    


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await GetBoundingBox(SvgElementReference);
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnInitialized()
    {
        moduleTask = new(async () => await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorSvgEditor.SvgEditor/svgEditor.js"));

        base.OnInitialized();
    }


    private void OnSvgPointerDown(PointerEventArgs e)
    {
        //Console.WriteLine("SVG POINTER DOWN: Offset: " + e.OffsetX + " " + e.OffsetY + "  Client: " + e.ClientX + " " + e.ClientY +  " Screen: " + e.ScreenX + " " + e.ScreenY  + "  Tilt: " + e.TiltX + " " + e.TiltY );
    }
    
    private string color = "red";
    private void OnElementPointerDown(PointerEventArgs e)
    {
        Console.WriteLine("ELEMENT POINTER DOWN: Offset: " + e.OffsetX + " " + e.OffsetY + "  Client: " + e.ClientX + " " + e.ClientY +  " Screen: " + e.ScreenX + " " + e.ScreenY  + "  Tilt: " + e.TiltX + " " + e.TiltY );
        color = "blue";
    }
    
    private void UnSelect(PointerEventArgs e)
    {
        color = "red";
    }


}