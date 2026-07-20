namespace CopperSkin.Core.Graphics;

/// <summary>
/// Represents one renderer-neutral graphic element.
/// </summary>
public sealed class GraphicElement
{
    /// <summary>
    /// Gets or sets the stable element identifier used for selection and history.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the element discriminator stored in the document.
    /// </summary>
    public GraphicElementKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the geometric payload for the selected element kind.
    /// </summary>
    public GraphicGeometry Geometry { get; set; } = new();

    /// <summary>
    /// Gets or sets the optional text content for a text element.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the fill, stroke, opacity, and token references.
    /// </summary>
    public GraphicStyle Style { get; set; } = new();

    /// <summary>
    /// Gets or sets the document-space transform.
    /// </summary>
    public GraphicTransform Transform { get; set; } = GraphicTransform.Identity;
}

/// <summary>
/// Identifies the vector/freehand element payload stored by v0.3.
/// </summary>
public enum GraphicElementKind
{
    /// <summary>Represents a path-command collection.</summary>
    Path,
    /// <summary>Represents a two-point line.</summary>
    Line,
    /// <summary>Represents an axis-aligned rectangle.</summary>
    Rectangle,
    /// <summary>Represents an axis-aligned ellipse.</summary>
    Ellipse,
    /// <summary>Represents a closed polygon.</summary>
    Polygon,
    /// <summary>Represents a freehand point stroke.</summary>
    Freehand,
    /// <summary>Represents editable text.</summary>
    Text
}

/// <summary>
/// Holds the geometry payload shared by the v0.3 element kinds.
/// </summary>
public sealed class GraphicGeometry
{
    /// <summary>
    /// Gets or sets the bounds used by rectangles, ellipses, and text.
    /// </summary>
    public GraphicRect Bounds { get; set; }

    /// <summary>
    /// Gets or sets the points used by lines, polygons, and freehand strokes.
    /// </summary>
    public List<GraphicPoint> Points { get; set; } = new();

    /// <summary>
    /// Gets or sets the commands used by a path.
    /// </summary>
    public List<GraphicPathCommand> Path { get; set; } = new();
}

/// <summary>
/// Represents one path command using document-space points.
/// </summary>
public sealed class GraphicPathCommand
{
    /// <summary>
    /// Gets or sets the path command kind.
    /// </summary>
    public GraphicPathCommandKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the first command point.
    /// </summary>
    public GraphicPoint Point1 { get; set; }

    /// <summary>
    /// Gets or sets the second command point for cubic curves.
    /// </summary>
    public GraphicPoint Point2 { get; set; }

    /// <summary>
    /// Gets or sets the third command point for cubic curves.
    /// </summary>
    public GraphicPoint Point3 { get; set; }
}

/// <summary>
/// Identifies a serializable path command.
/// </summary>
public enum GraphicPathCommandKind
{
    /// <summary>Moves the current path position.</summary>
    MoveTo,
    /// <summary>Adds a straight segment.</summary>
    LineTo,
    /// <summary>Adds a cubic Bézier segment.</summary>
    CubicTo,
    /// <summary>Closes the current subpath.</summary>
    Close
}
