using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.Misc;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Shapes;

public class Rectangle : Shape
{
    public Rectangle(SvgEditor svgEditor) : base(svgEditor){ }

    internal override Type Presenter => typeof(RectangleEditor);
    public override ShapeType ShapeType => ShapeType.Rectangle;

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private Coord<double> AddPosition = new(-1, -1);

    public override BoundingBox Bounds => new BoundingBox(X, Y, X + Width, Y + Height);

    internal override void SnapToInteger()
    {
        X = X.ToInt();
        Y = Y.ToInt();
        Width = Width.ToInt();
        Height = Height.ToInt();
    }
    
    bool isMoved = false;
    internal override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        Coord<double> resultCoord = BoundingBox.GetAvailableResultCoord(SvgEditor.ImageBoundingBox, point);
        
        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                
                if (AddPosition.X.IsEqual(-1)) AddPosition = new Coord<double>(X, Y);
                
                if (resultCoord.X < AddPosition.X)
                {
                    X = resultCoord.X;
                    Width = AddPosition.X - resultCoord.X;
                }
                else
                {
                    X = AddPosition.X;
                    Width = resultCoord.X - AddPosition.X;
                }
                if (resultCoord.Y < AddPosition.Y)
                {
                    Y = resultCoord.Y;
                    Height = AddPosition.Y - resultCoord.Y;
                }
                else
                {
                    Y = AddPosition.Y;
                    Height = resultCoord.Y - AddPosition.Y;
                }
                
                if (Width < 1) Width = 1;
                if (Height < 1) Height = 1;
                
                break;
            
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                var result = BoundingBox.GetAvailableMovingCoord(SvgEditor.ImageBoundingBox, Bounds, diff);

                X += result.X;
                Y += result.Y;
                
                isMoved = true;
                break;
            
            case EditMode.MoveAnchor:
                SvgEditor.SelectedAnchorIndex ??= 0;
                
                switch (SvgEditor.SelectedAnchorIndex)
                {
                    case 0:
                        Width -= resultCoord.X - X;
                        Height -= resultCoord.Y - Y;
                        X = resultCoord.X;
                        Y = resultCoord.Y;
                        break;
                    case 1:
                        Width = resultCoord.X - X;
                        Height -= resultCoord.Y - Y;
                        Y = resultCoord.Y;
                        break;
                    case 2:
                        Width = resultCoord.X - X;
                        Height = resultCoord.Y - Y;
                        break;
                    case 3:
                        Width -= resultCoord.X - X;
                        Height = resultCoord.Y - Y;
                        X = resultCoord.X;
                        break;
                }

                if (Width < 0)
                {
                    Width = -Width;
                    X -= Width;
                    if (SvgEditor.SelectedAnchorIndex == 0) SvgEditor.SelectedAnchorIndex = 1;
                    else if (SvgEditor.SelectedAnchorIndex == 1) SvgEditor.SelectedAnchorIndex = 0;
                    else if (SvgEditor.SelectedAnchorIndex == 2) SvgEditor.SelectedAnchorIndex = 3;
                    else if (SvgEditor.SelectedAnchorIndex == 3) SvgEditor.SelectedAnchorIndex = 2;
                }
                if (Height < 0)
                {
                    Height = -Height;
                    Y -= Height;
                    if (SvgEditor.SelectedAnchorIndex == 0) SvgEditor.SelectedAnchorIndex = 3;
                    else if (SvgEditor.SelectedAnchorIndex == 1) SvgEditor.SelectedAnchorIndex = 2;
                    else if (SvgEditor.SelectedAnchorIndex == 2) SvgEditor.SelectedAnchorIndex = 1;
                    else if (SvgEditor.SelectedAnchorIndex == 3) SvgEditor.SelectedAnchorIndex = 0;
                }
                
                if (Width < 1) Width = 1;
                if (Height < 1) Height = 1;
                
                break;
        }
    }

    internal override async Task HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Add)
        {
            if (Width < 1) Width = 1;
            if (Height < 1) Height = 1;
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
}