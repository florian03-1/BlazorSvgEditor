using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public partial class SvgEditor
{
    private const int MOVE_BUTTON_INDEX = 2;
    
    private Coord<int> _pointerPosition;

    //Container events (pointer events, wheel events)
    private async Task OnContainerPointerDown(PointerEventArgs e)
    {
        if (e.Button == MOVE_BUTTON_INDEX)
        {
            IsTranslating = true;
        }
        
        var point = DetransformPoint(e.OffsetX, e.OffsetY);
        if (point.X < 0 || point.Y < 0 || point.X > ImageSize.Width || point.Y > ImageSize.Height)
        {
            if (EditMode == EditMode.Add) return; //Wenn das Polygon erstellt wird und währenddessen aus dem Bild rausgezogen wird, soll nichts passieren
            OnUnselectPanelPointerDown(e);
        }
        else //Pointer is inside the image
        {
            if (EditMode == EditMode.AddTool)
            {
                await AddToolPointerDown(e);
            }
        }

        Console.WriteLine("OnContainerPointerDown");
    }
    
    private void OnContainerPointerUp(PointerEventArgs e)
    {
        IsTranslating = false;
        SelectedShape?.HandlePointerUp(e);
        SelectedShape?.SnapToInteger();
    }
    
    private void OnContainerPointerMove(PointerEventArgs e)
    {
        if(ShowDiagnosticInformation) _pointerPosition = new Coord<int>((int)e.OffsetX, (int) e.OffsetY);
        
        if (IsTranslating) Pan(e.MovementX, e.MovementY);

        if (SelectedShape != null)
        {
            SelectedShape.HandlePointerMove(e);
            MoveStartDPoint = DetransformOffset(e);
        }
    }
    
    private void OnContainerWheel(WheelEventArgs e)
    {
        //Zoom
        Zoom(e.DeltaY, e.OffsetX, e.OffsetY);
    }

    
    private void OnUnselectPanelPointerDown(PointerEventArgs e)
    {
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }

    private void OnContainerDoubleClick()
    {
        if (EditMode == EditMode.Add && SelectedShape != null)
        {
            //SelectedShape.Complete();
            //Führt aktuell selten zu Fehlern
        }
    }
}