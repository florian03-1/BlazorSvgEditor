using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public class Rectangle : Shape
{
    public Rectangle(SvgEditor svgEditor) : base(svgEditor)
    {
    }

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

        (double width, double height, double x, double y) oldData;
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                oldData = (Width, Height, X, Y); 

                if (AddPosition.X == -1) AddPosition = new Coord<double>(X, Y);
                
                if (point.X < AddPosition.X)
                {
                    X = point.X;
                    Width = AddPosition.X - point.X;
                }
                else
                {
                    X = AddPosition.X;
                    Width = point.X - AddPosition.X;
                }
                if (point.Y < AddPosition.Y)
                {
                    Y = point.Y;
                    Height = AddPosition.Y - point.Y;
                }
                else
                {
                    Y = AddPosition.Y;
                    Height = point.Y - AddPosition.Y;
                }
                
                if (ContainerBox.IsContainerFitInto(Bounds, SvgEditor.ImageBoundingBox) == false)
                {
                    X = oldData.x;
                    Y = oldData.y;
                    Width = oldData.width;
                    Height = oldData.height;
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
                
                //Lieber einen Test auf den Maximalen Wert der Erhöhung machen und wenn der Kreis zu groß wird, diesen Maximalen wert setzen!

                oldData = (Width, Height, X, Y); 
                
                SvgEditor.SelectedAnchorIndex ??= 0;
                
                switch (SvgEditor.SelectedAnchorIndex)
                {
                    case 0:
                        Width -= point.X - X;
                        Height -= point.Y - Y;
                        X = point.X;
                        Y = point.Y;
                        break;
                    case 1:
                        Width = point.X - X;
                        Height -= point.Y - Y;
                        Y = point.Y;
                        break;
                    case 2:
                        Width = point.X - X;
                        Height = point.Y - Y;
                        break;
                    case 3:
                        Width -= point.X - X;
                        Height = point.Y - Y;
                        X = point.X;
                        break;
                }

                if (ContainerBox.IsContainerFitInto(Bounds, SvgEditor.ImageBoundingBox) == false)
                {
                    X = oldData.x;
                    Y = oldData.y;
                    Width = oldData.width;
                    Height = oldData.height;
                }
                
                if (Width < 0)
                {
                    Width = 1;
                    X = oldData.x;
                }
                if (Height < 0)
                {
                    Height = 1;
                    Y = oldData.y;
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