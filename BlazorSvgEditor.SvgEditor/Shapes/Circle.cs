using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Shapes;

public class Circle : Shape
{

    public Circle(SvgEditor svgEditor) : base(svgEditor) { }
    
    internal override Type Presenter => typeof(CircleEditor);
    public override ShapeType ShapeType => ShapeType.Circle;

    //Own Properties
    public double Cx { get; set; }
    public double Cy { get; set; }
    public double R { get; set; }

    protected override BoundingBox Bounds => new BoundingBox(Cx -R, Cy - R, Cx + R, Cy + R);

    internal override void SnapToInteger()
    {
        Cx = Cx.ToInt();
        Cy = Cy.ToInt();
        R = R.ToInt();
    }
    
    bool isMoved = false;
    
    internal override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);

        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                var askedRadius = Math.Max(Math.Abs(point.X - Cx), Math.Abs(point.Y - Cy));
                R = GetMaxRadius(SvgEditor.ImageBoundingBox, new Coord<double>(Cx, Cy), askedRadius).Round();
                
                break;
            
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                var result = BoundingBox.GetAvailableMovingCoord(SvgEditor.ImageBoundingBox, Bounds, diff);

                Cx = (Cx + result.X).Round();
                Cy = (Cy + result.Y).Round();

                isMoved = true;
                break;
            case EditMode.MoveAnchor:
                
                SvgEditor.SelectedAnchorIndex ??= 0;
                
                switch (SvgEditor.SelectedAnchorIndex)
                { 
                    case 0:
                    case 1:
                        R = GetMaxRadius(SvgEditor.ImageBoundingBox, new Coord<double>(Cx, Cy), point.X - Cx).Round();
                        break;
                    case 2:
                    case 3:
                        R = GetMaxRadius(SvgEditor.ImageBoundingBox, new Coord<double>(Cx, Cy), point.Y - Cy).Round();
                        break;
                }
                
                if (R < 1) R = 1; //Mindestgröße des Kreises
                
                break;
        }
    }

    internal override async Task HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Add)
        {
            if (R == 0) R = GetMaxRadius(SvgEditor.ImageBoundingBox, new Coord<double>(Cx, Cy), 15); //Wenn Radius 0 ist, wurde der Kreis nur durch ein Klicken erzeugt, also wird er auf 15 gesetzt
            await Complete();
        }

        if (SvgEditor.EditMode == EditMode.Move && isMoved)
        {
            isMoved = false;
            await FireOnShapeChangedMove();
        }
        else if (SvgEditor.EditMode == EditMode.MoveAnchor) await FireOnShapeChangedEdit();
        
        SvgEditor.EditMode = EditMode.None;

    }

    internal override void HandlePointerOut(PointerEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }
    
    
    
    
    //Own BoundingBox Methods because Radius makes it more complicated
    private double GetMaxRadius(BoundingBox outerBox, Coord<double> centerCoord, double askedRadius)
    {
        var availableMovingValues = BoundingBox.GetAvailableMovingValues(outerBox, centerCoord);
        var maxRadius = Math.Min(Math.Min(availableMovingValues.Top, availableMovingValues.Left), Math.Min(availableMovingValues.Bottom, availableMovingValues.Right));
        
        if (Math.Abs(askedRadius) > maxRadius) return maxRadius;
        return Math.Abs(askedRadius);
    }
    
}