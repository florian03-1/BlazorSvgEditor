using BlazorSvgEditor.SvgEditor.Shapes;

namespace BlazorSvgEditor.SvgEditor;

public class ShapeChangedEventArgs : EventArgs
{
    public ShapeChangeType ChangeType { get; set; }
    public Shape? Shape { get; private set; } = null!;
    
    private Guid _shapeId;
    public Guid ShapeId => Shape?.CustomId ?? _shapeId;
    
    public static ShapeChangedEventArgs ShapeMoved(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Move,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeEdited(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Edit,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeAdded(Shape shape)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Add,
            Shape = shape
        };
    }
    
    public static ShapeChangedEventArgs ShapeDeleted(Guid deletedShapeId)
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.Delete,
            Shape = null!,
            _shapeId = deletedShapeId
        };
    }
    
    public static ShapeChangedEventArgs ShapesCleared()
    {
        return new ShapeChangedEventArgs()
        {
            ChangeType = ShapeChangeType.ClearAll,
            Shape = null!
        };
    }
}
public enum ShapeChangeType
{
    Move,
    Edit,
    Add,
    Delete,
    ClearAll,
    Other
}
