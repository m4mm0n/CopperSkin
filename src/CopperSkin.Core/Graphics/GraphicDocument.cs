namespace CopperSkin.Core.Graphics;

/// <summary>
/// Represents an editable CopperSkin icon or basic-paint document without WPF dependencies.
/// </summary>
public sealed class GraphicDocument
{
    /// <summary>
    /// The first public graphics-document schema version.
    /// </summary>
    public const int CurrentSchemaVersion = 1;

    /// <summary>
    /// Gets or sets the schema version used to serialize this document.
    /// </summary>
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    /// <summary>
    /// Gets or sets the stable document identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author-facing document name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the document is an icon or a basic-paint canvas.
    /// </summary>
    public GraphicDocumentType DocumentType { get; set; } = GraphicDocumentType.Icon;

    /// <summary>
    /// Gets or sets the document width in logical pixels.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the document height in logical pixels.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the canvas background color.
    /// </summary>
    public GraphicColor Background { get; set; } = GraphicColor.Transparent;

    /// <summary>
    /// Gets or sets the ordered layer collection, from back to front.
    /// </summary>
    public List<GraphicLayer> Layers { get; set; } = new();
}

/// <summary>
/// Identifies the authoring mode of a graphics document.
/// </summary>
public enum GraphicDocumentType
{
    /// <summary>
    /// The document is intended for icon-sized vector artwork.
    /// </summary>
    Icon,

    /// <summary>
    /// The document is intended for an open-ended basic-paint canvas.
    /// </summary>
    Paint
}

/// <summary>
/// Represents one ordered, editable graphics layer.
/// </summary>
public sealed class GraphicLayer
{
    /// <summary>
    /// Gets or sets the stable layer identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name shown by the editor.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the layer is rendered.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the editor may change the layer.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets the ordered element collection, from back to front.
    /// </summary>
    public List<GraphicElement> Elements { get; set; } = new();
}
