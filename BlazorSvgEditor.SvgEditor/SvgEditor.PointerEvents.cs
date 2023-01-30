using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    
    //Container events (pointer events, wheel events)
    public void OnContainerPointerDown(PointerEventArgs e)
    {
       IsTranslating = true;
    }
    
    public void OnContainerPointerUp(PointerEventArgs e)
    {
        IsTranslating = false;
    }
    
    public void OnContainerPointerMove(PointerEventArgs e)
    {
        //Wenn "Bewegungstaste gedrückt" -> Transformation
        if (IsTranslating) Pan(e.MovementX, e.MovementY);
    }
    
    public void OnContainerWheel(WheelEventArgs e)
    {
        //Zoom
        Zoom(e.DeltaY, e.OffsetX, e.OffsetY);
    }


    public void TestButton()
    {
        Console.WriteLine("TestButton");
        ResetTransformation();
    }
}