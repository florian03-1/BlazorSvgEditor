using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public class Circle : Shape
{

    public Circle(SvgEditor svgEditor) : base(svgEditor)
    {
    }
    
    public override Type Presenter => typeof(CircleEditor);

    
    //Own Properties
    public double Cx { get; set; }
    public double Cy { get; set; }
    public double R { get; set; }
    
    public override void SnapToInteger()
    {
        throw new NotImplementedException();
    }

    public override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        Console.WriteLine(SvgEditor.MoveStartDPoint);
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                break;
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                Cx += diff.X;
                Cy += diff.Y;
                break;
            case EditMode.MoveAnchor:
                break;
            case EditMode.Scale:
                break;
        }
    }

    public override void HandlePointerUp(PointerEventArgs eventArgs)
    {
        SvgEditor.EditMode = EditMode.None;
    }

    public override void HandlePointerOut(PointerEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    public override void Complete()
    {
        throw new NotImplementedException();
    }
}