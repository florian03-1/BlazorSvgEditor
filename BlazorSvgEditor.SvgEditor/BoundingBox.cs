using System.Diagnostics;
using BlazorSvgEditor.SvgEditor.Helper;

namespace BlazorSvgEditor.SvgEditor;

public struct JsBoundingBox
{
    public double Width { get; set; }

    public double Height { get; set; }
    
    public double X { get; set; }
    public double Y { get; set; }
    
}

public struct BoundingBox
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Right { get; set; }
    public double Bottom { get; set; }
    
    public double XPos => Right;
    public double YPos => Bottom;
    public double XNeg => Left;
    public double YNeg => Top;

    public double Width => Right - Left;
    public double Height => Bottom - Top;
    
    public BoundingBox(double left, double top, double right, double bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    
    public BoundingBox(double width, double height)
    {
        Left = 0;
        Top = 0;
        Right = width;
        Bottom = height;
    }
    
    public override string ToString()
    {
        return $"L: {Left.ToInvariantString()}, T: {Top.ToInvariantString()}, R: {Right.ToInvariantString()}, B: {Bottom.ToInvariantString()}";
    }
    
    public static BoundingBox GetAvailableMovingValues(BoundingBox outerBox, Coord<double> coords)
    {
        return new BoundingBox(
            coords.X,
            coords.Y,
            outerBox.Right - coords.X,
            outerBox.Bottom - coords.Y
        );
    }
    
    public static BoundingBox GetAvailableMovingValues(BoundingBox outerBox, Coord<int> coords)
    {
        return GetAvailableMovingValues(outerBox, (Coord<double>) coords);
    }
    
    public static BoundingBox GetAvailableMovingValues(BoundingBox outerBox, BoundingBox innerBox)
    {
        return new BoundingBox(
            innerBox.Left,
            innerBox.Top,
            outerBox.Right - innerBox.Right,
            outerBox.Bottom - innerBox.Bottom
        );
    }

    //Für 1.000.000 Aufrufe ca. 100ms (ohne Debug) - 1 Aufruf ca. 20ms
    public static Coord<double> GetAvailableMovingCoord(BoundingBox maxMovingValues, Coord<double> movingCoord)
    {
        Coord<double> result = new();
        
        if (movingCoord.X <= 0) //Moving left
        {
            if (Math.Abs(movingCoord.X) > maxMovingValues.Left) result.X = -maxMovingValues.Left;
            else result.X = movingCoord.X;
        }
        else //Moving right
        {
            if (movingCoord.X > maxMovingValues.Right) movingCoord.X = maxMovingValues.Right;
            result.X = movingCoord.X;
        }
        
        if (movingCoord.Y <= 0) //Moving up
        {
            if (Math.Abs(movingCoord.Y) > maxMovingValues.Top) result.Y = -maxMovingValues.Top;
            else result.Y = movingCoord.Y;
        }
        else //Moving down
        {
            if (movingCoord.Y > maxMovingValues.Bottom) movingCoord.Y = maxMovingValues.Bottom;
            result.Y = movingCoord.Y;
        }

        return result;
    }
    
    //Methode die AvailableMovingValues und AvaiableMoovingCoords vereint
    public static Coord<double> GetAvailableMovingCoord(BoundingBox outerBox, BoundingBox innerBox, Coord<double> movingCoord)
    {
        return GetAvailableMovingCoord(GetAvailableMovingValues(outerBox, innerBox), movingCoord);
    }
    public static Coord<double> GetAvailableMovingCoord(BoundingBox outerBox, Coord<double> coords, Coord<double> movingCoord)
    {
        return GetAvailableMovingCoord(GetAvailableMovingValues(outerBox, coords), movingCoord);
    }


    public static Coord<double> GetAvailableResultCoord(BoundingBox outerBox, Coord<double> coord)
    {
        var result = new Coord<double>();

        if (coord.X < outerBox.Left) result.X = outerBox.Left;
        else if (coord.X >= outerBox.Left && coord.X <= outerBox.Right) result.X = coord.X;
        else if (coord.X > outerBox.Right) result.X = outerBox.Right;
        
        if (coord.Y < outerBox.Top) result.Y = outerBox.Top;
        else if (coord.Y >= outerBox.Top && coord.Y <= outerBox.Bottom) result.Y = coord.Y;
        else if (coord.Y > outerBox.Bottom) result.Y = outerBox.Bottom;
        
        return result;
    }
}