namespace CopperSkin.Core.Graphics;

/// <summary>
/// Represents a renderer-neutral affine transform.
/// </summary>
public readonly record struct GraphicTransform(double X, double Y, double ScaleX, double ScaleY, double Rotation)
{
    /// <summary>
    /// Gets the identity transform.
    /// </summary>
    public static GraphicTransform Identity => new(0, 0, 1, 1, 0);
}

/// <summary>
/// Represents a point in document coordinates.
/// </summary>
public readonly record struct GraphicPoint(double X, double Y);

/// <summary>
/// Represents a rectangle in document coordinates.
/// </summary>
public readonly record struct GraphicRect(double X, double Y, double Width, double Height);
