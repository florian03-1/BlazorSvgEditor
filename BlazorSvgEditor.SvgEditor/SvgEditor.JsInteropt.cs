using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor : IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JsRuntime { get; set; }

    private Lazy<Task<IJSObjectReference>> moduleTask;
    
    
    
    public BoundingBox ContainerBoundingBox = new();
    public BoundingBox SvgBoundingBox = new();
    
    private async Task<BoundingBox> GetBoundingBox(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        
        return await module.InvokeAsync<BoundingBox>("getBoundingBox", elementReference);
    }
    
    public async Task SetContainerAndSvgBoundingBox()
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