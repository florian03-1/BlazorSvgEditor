using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor : IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!; //DI

    private Lazy<Task<IJSObjectReference>> moduleTask = null!; //wird gesetzt in OnInitialized
    
    
    
    private JsBoundingBox _containerBoundingBox = new();
    private ElementReference _containerElementReference;

    private async Task<JsBoundingBox> GetBoundingBox(ElementReference elementReference)
    {
        var module = await moduleTask.Value;
        
        return await module.InvokeAsync<JsBoundingBox>("getBoundingBox", elementReference);
    }
    
    private async Task SetContainerBoundingBox()
    {
        if (_containerElementReference.Id == null) throw new Exception("ContainerElementReference or SvgElementReference is null");
        
        _containerBoundingBox = await GetBoundingBox(_containerElementReference);
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