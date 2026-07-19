using System.Globalization;
using System.Windows;
using System.Windows.Media;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Renders Core graphics documents using WPF drawing primitives.
/// </summary>
public static class GraphicRenderer
{
    /// <summary>
    /// Draws a complete document at its logical document-space size.
    /// </summary>
    public static void Render(DrawingContext drawingContext, GraphicDocument document, DrawingThemeSnapshot theme)
    {
        if (drawingContext is null)
            throw new ArgumentNullException(nameof(drawingContext));
        if (document is null)
            throw new ArgumentNullException(nameof(document));
        if (theme is null)
            throw new ArgumentNullException(nameof(theme));

        var canvas = new Rect(0, 0, document.Width, document.Height);
        var background = CreateBrush(document.Background, null, theme);
        if (background is not null)
            drawingContext.DrawRectangle(background, null, canvas);

        foreach (var layer in document.Layers)
        {
            if (!layer.IsVisible)
                continue;

            foreach (var element in layer.Elements)
                RenderElement(drawingContext, element, theme);
        }
    }

    private static void RenderElement(DrawingContext drawingContext, GraphicElement element, DrawingThemeSnapshot theme)
    {
        var style = element.Style;
        var opacity = Math.Clamp(style.Opacity, 0, 1);
        if (opacity <= 0)
            return;

        var transform = CreateTransform(element.Transform);
        var hasTransform = transform is not null;
        if (hasTransform)
            drawingContext.PushTransform(transform);
        drawingContext.PushOpacity(opacity);

        var fill = CreateBrush(style.Fill, style.FillToken, theme);
        var pen = CreatePen(style.Stroke, style.StrokeToken, style.StrokeWidth, theme);
        switch (element.Kind)
        {
            case GraphicElementKind.Rectangle:
                drawingContext.DrawRectangle(fill, pen, ToRect(element.Geometry.Bounds));
                break;
            case GraphicElementKind.Ellipse:
                var ellipse = ToRect(element.Geometry.Bounds);
                drawingContext.DrawEllipse(fill, pen, new Point(ellipse.X + (ellipse.Width / 2), ellipse.Y + (ellipse.Height / 2)), ellipse.Width / 2, ellipse.Height / 2);
                break;
            case GraphicElementKind.Line:
                if (element.Geometry.Points.Count >= 2)
                    drawingContext.DrawLine(pen ?? new Pen(Brushes.Transparent, 0), ToPoint(element.Geometry.Points[0]), ToPoint(element.Geometry.Points[1]));
                break;
            case GraphicElementKind.Polygon:
            case GraphicElementKind.Freehand:
                DrawPoints(drawingContext, element.Geometry.Points, fill, pen, element.Kind == GraphicElementKind.Polygon);
                break;
            case GraphicElementKind.Path:
                DrawPath(drawingContext, element.Geometry.Path, fill, pen);
                break;
            case GraphicElementKind.Text:
                DrawText(drawingContext, element.Text, element.Geometry.Bounds, fill ?? Brushes.White);
                break;
        }

        drawingContext.Pop();
        if (hasTransform)
            drawingContext.Pop();
    }

    private static void DrawPath(DrawingContext drawingContext, IReadOnlyList<GraphicPathCommand> commands, Brush? fill, Pen? pen)
    {
        if (commands.Count == 0)
            return;

        var geometry = new StreamGeometry { FillRule = FillRule.Nonzero };
        using (var context = geometry.Open())
        {
            var hasFigure = false;
            foreach (var command in commands)
            {
                switch (command.Kind)
                {
                    case GraphicPathCommandKind.MoveTo:
                        context.BeginFigure(ToPoint(command.Point1), fill is not null, false);
                        hasFigure = true;
                        break;
                    case GraphicPathCommandKind.LineTo when hasFigure:
                        context.LineTo(ToPoint(command.Point1), true, false);
                        break;
                    case GraphicPathCommandKind.CubicTo when hasFigure:
                        context.BezierTo(ToPoint(command.Point1), ToPoint(command.Point2), ToPoint(command.Point3), true, false);
                        break;
                    case GraphicPathCommandKind.Close when hasFigure:
                        context.Close();
                        break;
                }
            }
        }
        geometry.Freeze();
        drawingContext.DrawGeometry(fill, pen, geometry);
    }

    private static void DrawPoints(DrawingContext drawingContext, IReadOnlyList<GraphicPoint> points, Brush? fill, Pen? pen, bool closed)
    {
        if (points.Count < 2)
            return;

        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(ToPoint(points[0]), closed && fill is not null, false);
            for (var i = 1; i < points.Count; i++)
                context.LineTo(ToPoint(points[i]), true, false);
            if (closed)
                context.Close();
        }
        geometry.Freeze();
        drawingContext.DrawGeometry(fill, pen, geometry);
    }

    private static void DrawText(DrawingContext drawingContext, string? text, GraphicRect bounds, Brush brush)
    {
        if (string.IsNullOrWhiteSpace(text) || bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var formatted = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            Math.Max(1, bounds.Height),
            brush,
            1.0);
        drawingContext.DrawText(formatted, new Point(bounds.X, bounds.Y));
    }

    private static Brush? CreateBrush(GraphicColor color, string? token, DrawingThemeSnapshot theme)
    {
        var tokenBrush = theme.GetTokenBrush(token);
        if (tokenBrush is not null)
            return tokenBrush;

        if (color.A == 0)
            return null;

        var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        brush.Freeze();
        return brush;
    }

    private static Pen? CreatePen(GraphicColor color, string? token, double width, DrawingThemeSnapshot theme)
    {
        if (color.A == 0 || width <= 0 || double.IsNaN(width) || double.IsInfinity(width))
            return null;

        var brush = CreateBrush(color, token, theme);
        if (brush is null)
            return null;
        var pen = new Pen(brush, width);
        pen.Freeze();
        return pen;
    }

    private static Transform? CreateTransform(GraphicTransform value)
    {
        if (value == GraphicTransform.Identity)
            return null;

        var group = new TransformGroup();
        if (value.ScaleX != 1 || value.ScaleY != 1)
            group.Children.Add(new ScaleTransform(value.ScaleX, value.ScaleY));
        if (value.Rotation != 0)
            group.Children.Add(new RotateTransform(value.Rotation));
        if (value.X != 0 || value.Y != 0)
            group.Children.Add(new TranslateTransform(value.X, value.Y));
        group.Freeze();
        return group;
    }

    private static Point ToPoint(GraphicPoint point) => new(point.X, point.Y);

    private static Rect ToRect(GraphicRect rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
}
