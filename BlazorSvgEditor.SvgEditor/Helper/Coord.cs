namespace BlazorSvgEditor.SvgEditor.Helper;

public struct Coord<T>
{
    public T X { get; set; }
    public T Y { get; set; }
    
    public Coord(T x, T y)
    {
        X = x;
        Y = y;
    }

    public Coord()
    {
        this = Zero;
    }

    public Coord(Coord<T> coord)
    {
        X = coord.X;
        Y = coord.Y;
    }
    
    public static Coord<T> Zero => new(default(T) ?? (dynamic)0,default(T)??(dynamic)0);

    public static Coord<T> operator +(Coord<T> a, Coord<T> b) => new((dynamic)a.X! + b.X, (dynamic)a.Y! + b.Y);
    public static Coord<T> operator -(Coord<T> a, Coord<T> b) => new((dynamic)a.X! - b.X, (dynamic)a.Y! - b.Y);
    
    public static bool operator ==(Coord<T> a, Coord<T> b) => (dynamic)a.X! == b.X && (dynamic)a.Y! == b.Y;
    public static bool operator !=(Coord<T> a, Coord<T> b) => !(a == b);
    
    public override bool Equals(object? obj) => obj is Coord<T> poT && this == poT;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    
    public override string ToString() => $"({X}, {Y})";
}


