using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public class Rectangle : Shape
{
    public Rectangle(SvgEditor svgEditor) : base(svgEditor){}

    public override Type Presenter => typeof(RectangleEditor);
    
    
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private Coord<double> AddPosition = new(-1, -1);

    public override ContainerBox Bounds =>
        new ContainerBox()
        {
            Top = (int)Y,
            Left = (int)X,
            Right = (int)(X + Width),
            Bottom = (int)(Y + Height)
        };

    public override void SnapToInteger()
    {
        X = X.ToInt();
        Y = Y.ToInt();
        Width = Width.ToInt();
        Height = Height.ToInt();
    }
    
    public override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        
        BoundingBox imageBB = new BoundingBox(SvgEditor.ImageSize.Width, SvgEditor.ImageSize.Height);

        (double width, double height, double x, double y) oldData;
        Coord<double> moovingCoord;
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                
                if (AddPosition.X.IsEqual(-1)) AddPosition = new Coord<double>(X, Y);

                moovingCoord = BoundingBox.GetAvailableResultCoord(imageBB, point);
                
                if (moovingCoord.X < AddPosition.X)
                {
                    X = moovingCoord.X;
                    Width = AddPosition.X - moovingCoord.X;
                }
                else
                {
                    X = AddPosition.X;
                    Width = moovingCoord.X - AddPosition.X;
                }
                if (moovingCoord.Y < AddPosition.Y)
                {
                    Y = moovingCoord.Y;
                    Height = AddPosition.Y - moovingCoord.Y;
                }
                else
                {
                    Y = AddPosition.Y;
                    Height = moovingCoord.Y - AddPosition.Y;
                }
                
                break;
            
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                var avaiableMovingCoords = ContainerBox.GetAvaiableMoovingCoords(Bounds, SvgEditor.ImageBoundingBox);
                var result = ContainerBox.GetAvaiableMovingCoordinates(avaiableMovingCoords, diff);

                X += result.X;
                Y += result.Y;
                break;
            
            case EditMode.MoveAnchor:
                moovingCoord = BoundingBox.GetAvailableResultCoord(imageBB, point);
                
                SvgEditor.SelectedAnchorIndex ??= 0;
                
                switch (SvgEditor.SelectedAnchorIndex)
                {
                    case 0:
                        Width -= moovingCoord.X - X;
                        Height -= moovingCoord.Y - Y;
                        X = moovingCoord.X;
                        Y = moovingCoord.Y;
                        break;
                    case 1:
                        Width = moovingCoord.X - X;
                        Height -= moovingCoord.Y - Y;
                        Y = moovingCoord.Y;
                        break;
                    case 2:
                        Width = moovingCoord.X - X;
                        Height = moovingCoord.Y - Y;
                        break;
                    case 3:
                        Width -= moovingCoord.X - X;
                        Height = moovingCoord.Y - Y;
                        X = moovingCoord.X;
                        break;
                }

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