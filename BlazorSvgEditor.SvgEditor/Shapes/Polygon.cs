using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor.Shapes;

public class Polygon : Shape
{
    public Polygon(SvgEditor svgEditor) : base(svgEditor){}

    internal override Type Presenter => typeof(PolygonEditor);
    public override ShapeType ShapeType => ShapeType.Polygon;

    //Own Properties

    public List<Coord<double>> Points { get; set; } = new();
    internal string PointsString => Points.Aggregate("", (current, point) => current + $"{point.X.ToInvString()},{point.Y.ToInvString()} ");
    
    private bool _addNewPointOnCreate = true;
    
    //Create Polygon Anchor Settings
    private double _polygonCompleteThreshold => 10;

    internal override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);
        Coord<double> resultCoord = BoundingBox.GetAvailableResultCoord(SvgEditor.ImageBoundingBox, point);

        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                if (_addNewPointOnCreate)
                {
                    _addNewPointOnCreate = false;
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
                    var newPoint = (p + result);
                    newPoints.Add(newPoint);
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
    

    internal override async Task HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Add)
        {
            if (Coord<double>.Distance(Points[^1], Points[0]) < _polygonCompleteThreshold && Points.Count > 3) //Es müssen mehr als 3 Punkte sein da gleich ja einer entfernt wird
            {
                //Ende des Polygonss
                Points.RemoveAt(Points.Count - 1);
                Complete();
            }
            if (!(Coord<double>.Distance(Points[^1], Points[0]) < _polygonCompleteThreshold)) //Die Punkte sind zu nah beieinander - keinen neuen Punkt im Polygon erstellen
            {
                _addNewPointOnCreate = true;
            }
        }
        else
        {
            if (SvgEditor.EditMode == EditMode.Move) await FireOnShapeChangedMove();
            else if (SvgEditor.EditMode == EditMode.MoveAnchor) await FireOnShapeChangedEdit();

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