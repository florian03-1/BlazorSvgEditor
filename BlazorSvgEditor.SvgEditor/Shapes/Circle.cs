using BlazorSvgEditor.SvgEditor.Helper;
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

    public override ContainerBox Bounds
    {
        get
        {
            return new ContainerBox()
            {
                Left = (int) (Cx - R),
                Top = (int) (Cy - R),
                Right = (int) (Cx + R),
                Bottom = (int) (Cy + R)
            };
        }
    }

    public override void SnapToInteger()
    {
        Cx = Cx.ToInt();
        Cy = Cy.ToInt();
        R = R.ToInt();
    }

    public override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        double rOld;
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                rOld = R;
                R = Math.Sqrt(Math.Pow(Cx - point.X, 2) + Math.Pow(Cy - point.Y, 2));
                if(ContainerBox.IsContainerFitInto(Bounds, SvgEditor.ImageBoundingBox) == false) R = rOld;
                break;
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                
                var avaiableMovingCoords = ContainerBox.GetAvaiableMoovingCoords(Bounds, SvgEditor.ImageBoundingBox);
                var result = ContainerBox.GetAvaiableMovingCoordinates(avaiableMovingCoords, diff);
                
                Cx += result.X;
                Cy += result.Y;
                
                break;
            case EditMode.MoveAnchor:
                
                //Lieber einen Test auf den Maximalen Wert der Erhöhung machen und wenn der Kreis zu groß wird, diesen Maximalen wert setzen!
                
                rOld = R;

                if (SvgEditor.SelectedAnchorIndex == null)
                {
                    SvgEditor.SelectedAnchorIndex = 0;
                }
                switch (SvgEditor.SelectedAnchorIndex)
                { 
                    case 0:
                    case 1:
                        R = Math.Abs(point.X - Cx);
                        break;
                    case 2:
                    case 3:
                        R = Math.Abs(point.Y - Cy);
                        break;
                }
                
                if(ContainerBox.IsContainerFitInto(Bounds, SvgEditor.ImageBoundingBox) == false) R = rOld;

                if (R < 1) R = 1; //Mindestgröße des Kreises
                
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