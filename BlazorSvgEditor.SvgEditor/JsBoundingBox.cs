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

public struct ContainerBox
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    
    public ContainerBox(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    
    public override string ToString()
    {
        return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
    }
    
    //Static Methods
    public static ContainerBox GetAvaiableMoovingCoords(ContainerBox innerBox, ContainerBox outerBox)
    {
        return new ContainerBox(
            innerBox.Left - outerBox.Left,
            innerBox.Top - outerBox.Top,
            outerBox.Right - innerBox.Right,
            outerBox.Bottom - innerBox.Bottom
        );
    }
    
    public static bool IsContainerFitInto(ContainerBox innerBox, ContainerBox outerBox)
    {
        return innerBox.Left >= outerBox.Left && innerBox.Top >= outerBox.Top && innerBox.Right <= outerBox.Right - 1 && innerBox.Bottom <= outerBox.Bottom - 1;
    }
    
    public static Coord<double> GetAvaiableMovingCoordinates(ContainerBox avaiableMovingValues, Coord<double> calculatedCoords)
    {
        Coord<double> res = new();
        
        if ((calculatedCoords.X >= 0 && calculatedCoords.X < avaiableMovingValues.Right) 
            || (calculatedCoords.X <= 0 && Math.Abs(calculatedCoords.X) < avaiableMovingValues.Left)) res.X = calculatedCoords.X;
        else res.X = calculatedCoords.X > 0 ? avaiableMovingValues.Right : avaiableMovingValues.Left;
        
        if ((calculatedCoords.Y >= 0 && calculatedCoords.Y < avaiableMovingValues.Bottom)
            || (calculatedCoords.Y <= 0 && Math.Abs(calculatedCoords.Y) < avaiableMovingValues.Top)) res.Y = calculatedCoords.Y;
        else res.Y = calculatedCoords.Y > 0 ? avaiableMovingValues.Bottom : avaiableMovingValues.Top;

        return res;
    }
    
    public static Coord<int> GetAvaiableMovingCoordinates(ContainerBox avaiableMovingValues, Coord<int> calculatedCoords)
    {
        return GetAvaiableMovingCoordinates(avaiableMovingValues, (Coord<double>) calculatedCoords);
    }


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