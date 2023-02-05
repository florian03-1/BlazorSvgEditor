namespace BlazorSvgEditor.SvgEditor;

public enum EditMode
{
    None,
    AddTool, //Add item but no start point is set
    Add,
    Move,
    MoveAnchor,
}

public enum ShapeType
{
    None,
    Polygon,
    Rectangle,
    Circle,
}

