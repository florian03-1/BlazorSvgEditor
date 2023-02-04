using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor : IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!; //DI

    private Lazy<Task<IJSObjectReference>> moduleTask = null!; //wird gesetzt in OnInitialized
    
    
    
    private JsBoundingBox ContainerBoundingBox = new();
    private JsBoundingBox SvgBoundingBox = new();
    
    private async Task<JsBoundingBox> GetBoundingBox(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        
        return await module.InvokeAsync<JsBoundingBox>("getBoundingBox", elementReference);
    }
    
    private async Task SetContainerAndSvgBoundingBox()
    {
        if (ContainerElementReference.Id == null || SvgGElementReference.Id == null) throw new Exception("ContainerElementReference or SvgElementReference is null");
        
        ContainerBoundingBox = await GetBoundingBox(ContainerElementReference);
        SvgBoundingBox = await GetBoundingBox(SvgGElementReference);
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