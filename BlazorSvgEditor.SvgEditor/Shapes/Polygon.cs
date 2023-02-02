using BlazorSvgEditor.SvgEditor.Helper;
using BlazorSvgEditor.SvgEditor.ShapeEditors;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSvgEditor.SvgEditor;

public class Polygon : Shape
{
    public Polygon(SvgEditor svgEditor) : base(svgEditor) {}

    public override Type Presenter => typeof(PolygonEditor);
    
    //Own Properties

    public List<Coord<double>> Points { get; set; } = new();
    public string PointsString => Points.Aggregate("", (current, point) => current + $"{point.X.ToInvariantString()},{point.Y.ToInvariantString()} ");
    
    public event Action<Shape> Changed;

    public override void HandlePointerMove(PointerEventArgs eventArgs)
    {
        var point = SvgEditor.DetransformPoint(eventArgs.OffsetX, eventArgs.OffsetY);

        switch (SvgEditor.EditMode)
        {
            case EditMode.Add:
                Points.Add(point);
                break;
            
            case EditMode.Move:
                var diff = (point - SvgEditor.MoveStartDPoint);
                var avaiableMovingCoords = ContainerBox.GetAvaiableMoovingCoords(Bounds, SvgEditor.ImageBoundingBox);
                var result = ContainerBox.GetAvaiableMovingCoordinates(avaiableMovingCoords, diff);

                List<Coord<double>> newPoints = new List<Coord<double>>();
                foreach (var p in Points)
                {
                    newPoints.Add(p + result);
                }
                Points = newPoints;
                
                Console.WriteLine(Bounds);
                Changed?.Invoke(this);
                break;
            case EditMode.MoveAnchor:
                
                Console.WriteLine("Anchor Index: " + SvgEditor.SelectedAnchorIndex);
                
                //Lieber einen Test auf den Maximalen Wert der Erhöhung machen und wenn der Kreis zu groß wird, diesen Maximalen wert setzen!
                
                SvgEditor.SelectedAnchorIndex ??= 0;

                if (SvgEditor.SelectedAnchorIndex < Points.Count) //wenn ja, dann ist es ein "echter" Anchor
                {
                    Points[SvgEditor.SelectedAnchorIndex.Value] = point;
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

    public override void HandlePointerUp(PointerEventArgs eventArgs)
    {
        if (SvgEditor.EditMode == EditMode.Add) SvgEditor.EditMode = EditMode.None;
            
            SvgEditor.EditMode = EditMode.None;
    }

    
    public override ContainerBox Bounds => new()
    {
        Left = Points.OrderBy(x => x.X).FirstOrDefault().X.ToInt(),
        Right = Points.OrderByDescending(x => x.X).FirstOrDefault().X.ToInt(),
        Top = Points.OrderBy(x => x.Y).FirstOrDefault().Y.ToInt(),
        Bottom = Points.OrderByDescending(x => x.Y).FirstOrDefault().Y.ToInt(),
    };
    
    public override void SnapToInteger()
    {
        Points.ForEach(x => { 
            x.X = x.X.ToInt();
            x.Y = x.Y.ToInt();
        });
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