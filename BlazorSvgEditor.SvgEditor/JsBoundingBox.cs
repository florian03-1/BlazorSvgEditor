using BlazorSvgEditor.SvgEditor.Helper;

namespace BlazorSvgEditor.SvgEditor;

public struct JsBoundingBox
{
    public double Width { get; set; }

    public double Height { get; set; }
    
    public double X { get; set; }
    public double Y { get; set; }
    
    public BoundingBox ToBoundingBox()
    {
        return new BoundingBox(X, Y, X + Width, Y + Height);
    }
    
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
    
    public BoundingBox(double left, double top, double right, double bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    
    public BoundingBox AddCoordinates(Coord<double> coord)
    {
        return new BoundingBox(Left + coord.X, Top + coord.Y, Right - coord.X, Bottom - coord.Y);
    }
    
    public override string ToString()
    {
        return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
    }
}