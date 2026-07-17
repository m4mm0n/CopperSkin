using System.Windows;
using System.Windows.Media;
using CopperSkin.Core.Graphics;
using CopperSkin.Wpf.Drawing;

namespace CopperSkin.Wpf.Controls;

/// <summary>
/// Displays a CopperSkin graphics document as a reusable themed icon surface.
/// </summary>
public sealed class CopperIcon : FrameworkElement, IThemeAwareDrawingSurface
{
    /// <summary>
    /// Identifies the Document dependency property.
    /// </summary>
    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
        nameof(Document), typeof(GraphicDocument), typeof(CopperIcon), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Identifies the AccessibleName dependency property.
    /// </summary>
    public static readonly DependencyProperty AccessibleNameProperty = DependencyProperty.Register(
        nameof(AccessibleName), typeof(string), typeof(CopperIcon), new FrameworkPropertyMetadata(string.Empty));

    private DrawingThemeSnapshot _theme = DrawingThemeSnapshot.Default;

    /// <summary>
    /// Initializes a reusable icon surface and registers it for live themes when loaded.
    /// </summary>
    public CopperIcon()
    {
        Width = 24;
        Height = 24;
        Loaded += (_, _) => DrawingThemeRegistry.Register(this);
        Unloaded += (_, _) => DrawingThemeRegistry.Unregister(this);
    }

    /// <summary>
    /// Gets or sets the graphics document displayed by the icon.
    /// </summary>
    public GraphicDocument? Document
    {
        get => (GraphicDocument?)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    /// <summary>
    /// Gets or sets the accessible name exposed to host accessibility tooling.
    /// </summary>
    public string AccessibleName
    {
        get => (string)GetValue(AccessibleNameProperty);
        set => SetValue(AccessibleNameProperty, value);
    }

    /// <summary>
    /// Applies a drawing theme and invalidates the icon.
    /// </summary>
    public void ApplyTheme(DrawingThemeSnapshot theme)
    {
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        InvalidateVisual();
    }

    /// <summary>
    /// Draws the document uniformly inside the arranged icon bounds.
    /// </summary>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var document = Document;
        if (document is null || document.Width <= 0 || document.Height <= 0 || ActualWidth <= 0 || ActualHeight <= 0)
            return;

        var scale = Math.Min(ActualWidth / document.Width, ActualHeight / document.Height);
        var offsetX = (ActualWidth - (document.Width * scale)) / 2;
        var offsetY = (ActualHeight - (document.Height * scale)) / 2;
        drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
        drawingContext.PushTransform(new ScaleTransform(scale, scale));
        GraphicRenderer.Render(drawingContext, document, _theme);
        drawingContext.Pop();
        drawingContext.Pop();
    }
}
