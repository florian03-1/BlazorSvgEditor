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
        
        var point = DetransformPoint(e.OffsetX, e.OffsetY);
        if (point.X < 0 || point.Y < 0 || point.X > ImageSize.Width || point.Y > ImageSize.Height)
        {
            OnUnselectPanelPointerDown(e);
        }
        else //Pointer is inside the image
        {
            if (EditMode == EditMode.AddTool)
            {
                switch (ShapeType)
                {
                    case ShapeType.None:
                        break;
                    case ShapeType.Polygon:
                        break;
                    case ShapeType.Rectangle:
                        break;
                    case ShapeType.Circle:
                        var circle = new Circle(this);
        
                        circle.Cx = DetransformOffset(e).X;
                        circle.Cy = DetransformOffset(e).Y;

                        Shapes.Add(circle);
                        SelectedShape = circle;
                        EditMode = EditMode.Add;
                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        Console.WriteLine("OnContainerPointerDown");
    }
    
    public void OnContainerPointerUp(PointerEventArgs e)
    {
        IsTranslating = false;
        SelectedShape?.HandlePointerUp(e);
        EditMode = EditMode.None;
    }
    
    public void OnContainerPointerMove(PointerEventArgs e)
    {
        _pointerPosition = new Coord<int>((int)e.OffsetX, (int) e.OffsetY);
        if (IsTranslating) Pan(e.MovementX, e.MovementY);

        if (SelectedShape != null)
        {
            SelectedShape.HandlePointerMove(e);
            MoveStartDPoint = DetransformOffset(e);
        }
        

    }
    
    public void OnContainerWheel(WheelEventArgs e)
    {
        //Zoom
        Zoom(e.DeltaY, e.OffsetX, e.OffsetY);
    }

    
    private void OnUnselectPanelPointerDown(PointerEventArgs e)
    {
        SelectedShape?.UnSelectShape();
        SelectedShape = null;
    }
    

    public async void TestButton()
    {
        await SetContainerAndSvgBoundingBox();
        
        Console.WriteLine("TestButton");
        
        ResetTransformation();
    }
}