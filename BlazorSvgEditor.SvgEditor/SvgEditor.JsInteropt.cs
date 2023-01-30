using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor : IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JsRuntime { get; set; }

    private Lazy<Task<IJSObjectReference>> moduleTask;
    
    BoundingBox BoundingBox = new();
    
    public async Task<(double width, double height)> GetBoundingBox(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        
        var widthHeightJson = await module.InvokeAsync<JsonElement>("getElementWidthAndHeight", elementReference);
        var widthHeight = (widthHeightJson.GetProperty("width").GetDouble(), widthHeightJson.GetProperty("height").GetDouble());
        
        Console.WriteLine(widthHeight);
        BoundingBox = new BoundingBox(){Height = widthHeight.Item2, Width = widthHeight.Item1};

        return widthHeight;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            IJSObjectReference module = await moduleTask.Value;
            await module.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}