namespace CopperSkin.Core.Graphics;

/// <summary>
/// Describes fill, stroke, opacity, and optional theme-token references.
/// </summary>
public sealed class GraphicStyle
{
    /// <summary>
    /// Gets or sets the literal fill color.
    /// </summary>
    public GraphicColor Fill { get; set; } = GraphicColor.Transparent;

    /// <summary>
    /// Gets or sets an optional theme token that overrides the literal fill at render time.
    /// </summary>
    public string? FillToken { get; set; }

    /// <summary>
    /// Gets or sets the literal stroke color.
    /// </summary>
    public GraphicColor Stroke { get; set; } = GraphicColor.Transparent;

    /// <summary>
    /// Gets or sets an optional theme token that overrides the literal stroke at render time.
    /// </summary>
    public string? StrokeToken { get; set; }

    /// <summary>
    /// Gets or sets the stroke width in document units.
    /// </summary>
    public double StrokeWidth { get; set; } = 1;

    /// <summary>
    /// Gets or sets the element opacity from zero through one.
    /// </summary>
    public double Opacity { get; set; } = 1;
}

/// <summary>
/// Represents an ARGB color without depending on WPF or a UI framework.
/// </summary>
public readonly record struct GraphicColor(byte A, byte R, byte G, byte B)
{
    /// <summary>
    /// Gets a fully transparent color.
    /// </summary>
    public static GraphicColor Transparent => new(0, 0, 0, 0);

    /// <summary>
    /// Gets a fully opaque black color.
    /// </summary>
    public static GraphicColor Black => new(255, 0, 0, 0);
}
