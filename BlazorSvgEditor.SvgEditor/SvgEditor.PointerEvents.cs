using BlazorSvgEditor.SvgEditor.Helper;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private const int MOVE_BUTTON_INDEX = 2;
    
    private Coord<int> _pointerPosition;

    //Container events (pointer events, wheel events)
    public void OnContainerPointerDown(PointerEventArgs e)
    {
        if (e.Button == MOVE_BUTTON_INDEX)
        {
            IsTranslating = true;
        }
        EditMode = EditMode.Move;
        
        Console.WriteLine("OnContainerPointerDown");
    }
    
    public void OnContainerPointerUp(PointerEventArgs e)
    {
        IsTranslating = false;
        SelectedShape?.HandlePointerUp(e);
    }
    
    public void OnContainerPointerMove(PointerEventArgs e)
    {
        _pointerPosition = new Coord<int>((int)e.OffsetX, (int) e.OffsetY);
        if (IsTranslating) Pan(e.MovementX, e.MovementY);
        SelectedShape?.HandlePointerMove(e);
    }
    
    public void OnContainerWheel(WheelEventArgs e)
    {
        //Zoom
        Zoom(e.DeltaY, e.OffsetX, e.OffsetY);
    }


    public async void TestButton()
    {
        await SetContainerAndSvgBoundingBox();
        
        Console.WriteLine("TestButton");
        
        ResetTransformation();
    }
}