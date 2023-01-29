namespace BlazorSvgEditor.SvgEditor.Helper;

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point()
    {
        X = 0;
        Y = 0;
    }

    public Point(Point point)
    {
        X = point.X;
        Y = point.Y;
    }
    
    public static Point Zero => new(0, 0);

    public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public static Point operator *(Point a, int b) => new(a.X * b, a.Y * b);
    public static Point operator /(Point a, int b) => new(a.X / b, a.Y / b);
    
    public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
    
    public override bool Equals(object? obj) => obj is Point point && this == point;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    
    public override string ToString() => $"({X}, {Y})";
}