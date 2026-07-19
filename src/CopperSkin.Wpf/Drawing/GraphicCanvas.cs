using System.Windows;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Displays and theme-updates a Core graphics document in document coordinates.
/// </summary>
public class GraphicCanvas : FrameworkElement, IThemeAwareDrawingSurface
{
    /// <summary>
    /// Identifies the Document dependency property.
    /// </summary>
    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
        nameof(Document), typeof(GraphicDocument), typeof(GraphicCanvas), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Identifies the Zoom dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
        nameof(Zoom), typeof(double), typeof(GraphicCanvas), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    private DrawingThemeSnapshot _theme = DrawingThemeSnapshot.Default;

    /// <summary>
    /// Initializes a graphics canvas and registers it for live theme updates when loaded.
    /// </summary>
    public GraphicCanvas()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Gets or sets the document to render.
    /// </summary>
    public GraphicDocument? Document
    {
        get => (GraphicDocument?)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    /// <summary>
    /// Gets or sets the document-space zoom multiplier.
    /// </summary>
    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    /// <summary>
    /// Applies a drawing theme and invalidates the surface.
    /// </summary>
    public void ApplyTheme(DrawingThemeSnapshot theme)
    {
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        InvalidateVisual();
    }

    /// <summary>
    /// Reports the zoomed document extent so a parent <see cref="System.Windows.Controls.ScrollViewer"/> can scroll the whole canvas.
    /// </summary>
    protected override Size MeasureOverride(Size availableSize)
    {
        var document = Document;
        if (document is null)
            return base.MeasureOverride(availableSize);

        var zoom = double.IsFinite(Zoom) && Zoom > 0 ? Zoom : 1;
        return new Size(Math.Max(0, document.Width * zoom), Math.Max(0, document.Height * zoom));
    }

    /// <summary>
    /// Draws the current document with the configured zoom.
    /// </summary>
    protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var document = Document;
        if (document is null || document.Width <= 0 || document.Height <= 0)
            return;

        var zoom = double.IsFinite(Zoom) && Zoom > 0 ? Zoom : 1;
        drawingContext.PushTransform(new System.Windows.Media.ScaleTransform(zoom, zoom));
        GraphicRenderer.Render(drawingContext, document, _theme);
        drawingContext.Pop();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => DrawingThemeRegistry.Register(this);

    private void OnUnloaded(object sender, RoutedEventArgs e) => DrawingThemeRegistry.Unregister(this);
}
