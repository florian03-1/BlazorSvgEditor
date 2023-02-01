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
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                break;
            case EditMode.Move:
                
                Console.WriteLine(eventArgs.OffsetX);
                
                var relationWidth = SvgEditor.ImageSize.Width / SvgEditor.ContainerBoundingBox.Width;
                var relationHeight = SvgEditor.ImageSize.Height / SvgEditor.SvgBoundingBox.Height;
                
                Console.WriteLine($"relationWidth: {relationWidth}, relationHeight: {relationHeight}");
                
                Cx += eventArgs.MovementX * relationWidth;
                Cy += eventArgs.MovementY * relationHeight;
                Console.WriteLine($"Cx: {Cx}, Cy: {Cy}");
                break;
            case EditMode.MoveAnchor:
                break;
            case EditMode.Scale:
                break;
        }
    }

    public override void HandlePointerUp(PointerEventArgs eventArgs)
    {
       // throw new NotImplementedException();
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