using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public class Polygon : Shape
{
    public Polygon(SvgEditor svgEditor) : base(svgEditor) {}

    internal override Type Presenter => typeof(PolygonEditor);
    
    //Own Properties

    public List<Coord<double>> Points { get; set; } = new();
    internal string PointsString => Points.Aggregate("", (current, point) => current + $"{point.X.ToInvariantString()},{point.Y.ToInvariantString()} ");
    
    private bool _firstAdd = true;
    
    //Create Polygon Anchor Settings
    private double _polygonCompleteThreshold => 10;

    internal override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        Coord<double> resultCoord = BoundingBox.GetAvailableResultCoord(SvgEditor.ImageBoundingBox, point);

        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                if (_firstAdd)
                {
                    _firstAdd = false;
                    Points.Add(point);
                }
                
                Points[^1] = resultCoord;

                break;

            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                var result = BoundingBox.GetAvailableMovingCoord(SvgEditor.ImageBoundingBox, Bounds, diff);

                List<Coord<double>> newPoints = new List<Coord<double>>();
                foreach (var p in Points)
                {
                    newPoints.Add(p + result);
                }
                Points = newPoints;
                
                break;
            case EditMode.MoveAnchor:
                
                SvgEditor.SelectedAnchorIndex ??= 0;

                if (SvgEditor.SelectedAnchorIndex < Points.Count) //wenn ja, dann ist es ein "echter" Anchor
                {
                    Points[SvgEditor.SelectedAnchorIndex.Value] = resultCoord;
                }
                else
                {
                    int index = SvgEditor.SelectedAnchorIndex.Value - Points.Count + 1;
                    var coord = new Coord<double>(point);
                    Points.Insert(index, coord);
                    SvgEditor.SelectedAnchorIndex = index;
                }
                break;
        }    
    }

    //Delete Point
    internal void OnAnchorDoubleClicked(int anchorIndex)
    {
        if (SvgEditor.EditMode == EditMode.Add) return;
        if (Points.Count <= 3) return;  //Mindestens 3 Punkte für ein Polygon
        if (anchorIndex < Points.Count) //wenn ja, dann ist es ein "echter" Anchor
        {
            Points.RemoveAt(anchorIndex);
        }
    }
    

    internal override void HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Add)
        {
            if (Coord<double>.Distance(Points[^1], Points[0]) < _polygonCompleteThreshold && Points.Count > 2)
            {
                //Ende des Polygonss
                Points.RemoveAt(Points.Count - 1);
                Complete();
            }

            _firstAdd = true;
        }
        else
        {
            SvgEditor.EditMode = EditMode.None;
        }
    }

    
    protected override BoundingBox Bounds => new()
    {
        Left = Points.OrderBy(x => x.X).FirstOrDefault().X,
        Right = Points.OrderByDescending(x => x.X).FirstOrDefault().X,
        Top = Points.OrderBy(x => x.Y).FirstOrDefault().Y,
        Bottom = Points.OrderByDescending(x => x.Y).FirstOrDefault().Y,
    };
    
    internal override void SnapToInteger()
    {
        List<Coord<double>> newPoints = new List<Coord<double>>();
        foreach (var p in Points)
        {
            newPoints.Add(new Coord<double>(p.X.ToInt(), p.Y.ToInt()));
        }
        Points = newPoints;
    }

    
    
    internal override void HandlePointerOut(PointerEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    internal override void Complete()
    {
        SvgEditor.EditMode = EditMode.None;
    }
}