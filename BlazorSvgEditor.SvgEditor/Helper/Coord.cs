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
    
    
    public static Coord<T> Max(Coord<T> a, Coord<T> b) => new(Math.Max((dynamic)a.X!, (dynamic)b.X!), Math.Max((dynamic)a.Y!, (dynamic)b.Y!));
    
    //Cast to Coord<int>
    public static implicit operator Coord<int>(Coord<T> coord) => new((int)(dynamic)coord.X!, (int)(dynamic)coord.Y!);
    public static implicit operator Coord<double>(Coord<T> coord) => new((double)(dynamic)coord.X!, (double)(dynamic)coord.Y!);
}


