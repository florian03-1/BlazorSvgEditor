using Microsoft.AspNetCore.Components;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    [Parameter] 
    public (int width, int height) ImageSize { get; set; } = (700, 394);
    
    [Parameter]
    public string ImageSource { get; set; } = "https://www.bentleymotors.com/content/dam/bentley/Master/World%20of%20Bentley/Mulliner/redesign/coachbuilt/Mulliner%20Batur%201920x1080.jpg/_jcr_content/renditions/original.image_file.700.394.file/Mulliner%20Batur%201920x1080.jpg";//700 x 394
}