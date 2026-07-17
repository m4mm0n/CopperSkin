using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Exports validated Core graphics documents through the shared WPF rendering boundary.
/// </summary>
public static class GraphicExportService
{
    /// <summary>
    /// Serializes a graphics document as deterministic SVG.
    /// </summary>
    public static string ToSvg(GraphicDocument document)
    {
        EnsureValid(document);
        var builder = new StringBuilder();
        builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        builder.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"").Append(Number(document.Width)).Append("\" height=\"").Append(Number(document.Height)).Append("\" viewBox=\"0 0 ").Append(Number(document.Width)).Append(' ').Append(Number(document.Height)).Append("\">");
        var background = ColorValue(document.Background);
        if (document.Background.A > 0)
            builder.Append("<rect x=\"0\" y=\"0\" width=\"").Append(Number(document.Width)).Append("\" height=\"").Append(Number(document.Height)).Append("\" fill=\"").Append(background).Append("\" fill-opacity=\"").Append(Opacity(document.Background.A)).Append("\"/>");

        foreach (var layer in document.Layers)
        {
            if (!layer.IsVisible)
                continue;
            builder.Append("<g id=\"").Append(Escape(layer.Id)).Append("\">");
            foreach (var element in layer.Elements)
                AppendSvgElement(builder, element);
            builder.Append("</g>");
        }

        builder.Append("</svg>");
        return builder.ToString();
    }

    /// <summary>
    /// Serializes a graphics document as a WPF DrawingImage resource.
    /// </summary>
    public static string ToXaml(GraphicDocument document)
    {
        EnsureValid(document);
        var builder = new StringBuilder();
        builder.Append("<DrawingImage xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><DrawingImage.Drawing><DrawingGroup>");
        foreach (var layer in document.Layers)
        {
            if (!layer.IsVisible)
                continue;
            foreach (var element in layer.Elements)
                AppendXamlElement(builder, element);
        }
        builder.Append("</DrawingGroup></DrawingImage.Drawing></DrawingImage>");
        return builder.ToString();
    }

    /// <summary>
    /// Renders a graphics document to a PNG file with the requested pixel dimensions.
    /// </summary>
    public static void ExportPng(GraphicDocument document, string path, int pixelWidth, int pixelHeight, DrawingThemeSnapshot? theme = null)
    {
        EnsureValid(document);
        if (pixelWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelWidth));
        if (pixelHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelHeight));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("An output path is required.", nameof(path));

        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            context.PushTransform(new ScaleTransform(pixelWidth / document.Width, pixelHeight / document.Height));
            GraphicRenderer.Render(context, document, theme ?? DrawingThemeSnapshot.Default);
            context.Pop();
        }

        var bitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(path);
        encoder.Save(stream);
    }

    private static void AppendSvgElement(StringBuilder builder, GraphicElement element)
    {
        var style = element.Style;
        var common = new StringBuilder();
        common.Append(" fill=\"").Append(style.Fill.A == 0 ? "none" : ColorValue(style.Fill)).Append("\"");
        if (style.Fill.A > 0)
            common.Append(" fill-opacity=\"").Append(Opacity(style.Fill.A)).Append("\"");
        common.Append(" stroke=\"").Append(style.Stroke.A == 0 ? "none" : ColorValue(style.Stroke)).Append("\"");
        if (style.Stroke.A > 0)
            common.Append(" stroke-opacity=\"").Append(Opacity(style.Stroke.A)).Append("\" stroke-width=\"").Append(Number(style.StrokeWidth)).Append("\"");
        if (style.Opacity < 1)
            common.Append(" opacity=\"").Append(Number(style.Opacity)).Append("\"");
        var transform = SvgTransform(element.Transform);
        if (transform.Length > 0)
            common.Append(" transform=\"").Append(transform).Append("\"");

        switch (element.Kind)
        {
            case GraphicElementKind.Rectangle:
                var rect = element.Geometry.Bounds;
                builder.Append("<rect x=\"").Append(Number(rect.X)).Append("\" y=\"").Append(Number(rect.Y)).Append("\" width=\"").Append(Number(rect.Width)).Append("\" height=\"").Append(Number(rect.Height)).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Ellipse:
                var ellipse = element.Geometry.Bounds;
                builder.Append("<ellipse cx=\"").Append(Number(ellipse.X + (ellipse.Width / 2))).Append("\" cy=\"").Append(Number(ellipse.Y + (ellipse.Height / 2))).Append("\" rx=\"").Append(Number(ellipse.Width / 2)).Append("\" ry=\"").Append(Number(ellipse.Height / 2)).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Line when element.Geometry.Points.Count >= 2:
                builder.Append("<line x1=\"").Append(Number(element.Geometry.Points[0].X)).Append("\" y1=\"").Append(Number(element.Geometry.Points[0].Y)).Append("\" x2=\"").Append(Number(element.Geometry.Points[1].X)).Append("\" y2=\"").Append(Number(element.Geometry.Points[1].Y)).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Polygon:
                builder.Append("<polygon points=\"").Append(string.Join(" ", element.Geometry.Points.Select(point => Number(point.X) + "," + Number(point.Y)))).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Freehand:
                builder.Append("<polyline points=\"").Append(string.Join(" ", element.Geometry.Points.Select(point => Number(point.X) + "," + Number(point.Y)))).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Path:
                builder.Append("<path d=\"").Append(Escape(PathData(element.Geometry.Path))).Append('\"').Append(common).Append("/>");
                break;
            case GraphicElementKind.Text:
                var textBounds = element.Geometry.Bounds;
                builder.Append("<text x=\"").Append(Number(textBounds.X)).Append("\" y=\"").Append(Number(textBounds.Y + textBounds.Height)).Append("\" font-size=\"").Append(Number(textBounds.Height)).Append('\"').Append(common).Append('>').Append(Escape(element.Text ?? string.Empty)).Append("</text>");
                break;
        }
    }

    private static void AppendXamlElement(StringBuilder builder, GraphicElement element)
    {
        if (element.Kind == GraphicElementKind.Line && element.Geometry.Points.Count < 2)
            return;
        if ((element.Kind == GraphicElementKind.Polygon || element.Kind == GraphicElementKind.Freehand || element.Kind == GraphicElementKind.Path) && element.Geometry.Points.Count == 0 && element.Geometry.Path.Count == 0)
            return;
        builder.Append("<GeometryDrawing");
        if (element.Style.Fill.A > 0)
            builder.Append(" Brush=\"").Append(ColorValue(element.Style.Fill, includeAlpha: true)).Append('\"');
        builder.Append('>');
        if (element.Style.Stroke.A > 0 && element.Style.StrokeWidth > 0)
            builder.Append("<GeometryDrawing.Pen><Pen Brush=\"").Append(ColorValue(element.Style.Stroke, includeAlpha: true)).Append("\" Thickness=\"").Append(Number(element.Style.StrokeWidth)).Append("\"/></GeometryDrawing.Pen>");
        builder.Append("<GeometryDrawing.Geometry>");
        AppendXamlGeometry(builder, element);
        builder.Append("</GeometryDrawing.Geometry></GeometryDrawing>");
    }

    private static void AppendXamlGeometry(StringBuilder builder, GraphicElement element)
    {
        var bounds = element.Geometry.Bounds;
        switch (element.Kind)
        {
            case GraphicElementKind.Rectangle:
                builder.Append("<RectangleGeometry Rect=\"").Append(Number(bounds.X)).Append(',').Append(Number(bounds.Y)).Append(',').Append(Number(bounds.Width)).Append(',').Append(Number(bounds.Height)).Append("\"/>");
                break;
            case GraphicElementKind.Ellipse:
                builder.Append("<EllipseGeometry Center=\"").Append(Number(bounds.X + (bounds.Width / 2))).Append(',').Append(Number(bounds.Y + (bounds.Height / 2))).Append("\" RadiusX=\"").Append(Number(bounds.Width / 2)).Append("\" RadiusY=\"").Append(Number(bounds.Height / 2)).Append("\"/>");
                break;
            case GraphicElementKind.Line:
                builder.Append("<LineGeometry StartPoint=\"").Append(Number(element.Geometry.Points[0].X)).Append(',').Append(Number(element.Geometry.Points[0].Y)).Append("\" EndPoint=\"").Append(Number(element.Geometry.Points[1].X)).Append(',').Append(Number(element.Geometry.Points[1].Y)).Append("\"/>");
                break;
            case GraphicElementKind.Polygon:
            case GraphicElementKind.Freehand:
                builder.Append("<PathGeometry Figures=\"").Append(Escape(PathData(PointsAsPath(element.Geometry.Points, element.Kind == GraphicElementKind.Polygon)))).Append("\"/>");
                break;
            case GraphicElementKind.Path:
                builder.Append("<PathGeometry Figures=\"").Append(Escape(PathData(element.Geometry.Path))).Append("\"/>");
                break;
            case GraphicElementKind.Text:
                builder.Append("<PathGeometry Figures=\"").Append(Escape(TextGeometry(element.Text, bounds))).Append("\"/>");
                break;
        }
    }

    private static string TextGeometry(string? text, GraphicRect bounds)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        var formatted = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), Math.Max(1, bounds.Height), Brushes.White, 1.0);
        return formatted.BuildGeometry(new Point(bounds.X, bounds.Y)).ToString(CultureInfo.InvariantCulture);
    }

    private static List<GraphicPathCommand> PointsAsPath(IReadOnlyList<GraphicPoint> points, bool close)
    {
        var commands = new List<GraphicPathCommand>();
        if (points.Count == 0)
            return commands;
        commands.Add(new GraphicPathCommand { Kind = GraphicPathCommandKind.MoveTo, Point1 = points[0] });
        for (var i = 1; i < points.Count; i++)
            commands.Add(new GraphicPathCommand { Kind = GraphicPathCommandKind.LineTo, Point1 = points[i] });
        if (close)
            commands.Add(new GraphicPathCommand { Kind = GraphicPathCommandKind.Close });
        return commands;
    }

    private static string PathData(IReadOnlyList<GraphicPathCommand> commands)
    {
        var builder = new StringBuilder();
        foreach (var command in commands)
        {
            if (builder.Length > 0)
                builder.Append(' ');
            switch (command.Kind)
            {
                case GraphicPathCommandKind.MoveTo:
                    builder.Append('M').Append(' ').Append(Number(command.Point1.X)).Append(',').Append(Number(command.Point1.Y));
                    break;
                case GraphicPathCommandKind.LineTo:
                    builder.Append('L').Append(' ').Append(Number(command.Point1.X)).Append(',').Append(Number(command.Point1.Y));
                    break;
                case GraphicPathCommandKind.CubicTo:
                    builder.Append('C').Append(' ').Append(Number(command.Point1.X)).Append(',').Append(Number(command.Point1.Y)).Append(' ').Append(Number(command.Point2.X)).Append(',').Append(Number(command.Point2.Y)).Append(' ').Append(Number(command.Point3.X)).Append(',').Append(Number(command.Point3.Y));
                    break;
                case GraphicPathCommandKind.Close:
                    builder.Append('Z');
                    break;
            }
        }
        return builder.ToString();
    }

    private static string SvgTransform(GraphicTransform transform)
    {
        var values = new List<string>();
        if (transform.X != 0 || transform.Y != 0)
            values.Add($"translate({Number(transform.X)} {Number(transform.Y)})");
        if (transform.ScaleX != 1 || transform.ScaleY != 1)
            values.Add($"scale({Number(transform.ScaleX)} {Number(transform.ScaleY)})");
        if (transform.Rotation != 0)
            values.Add($"rotate({Number(transform.Rotation)})");
        return string.Join(" ", values);
    }

    private static string ColorValue(GraphicColor color, bool includeAlpha = false)
    {
        return includeAlpha
            ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
            : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string Number(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static string Opacity(byte alpha) => (alpha / 255d).ToString("0.###", CultureInfo.InvariantCulture);

    private static string Escape(string value) => value.Replace("&", "&amp;", StringComparison.Ordinal).Replace("\"", "&quot;", StringComparison.Ordinal).Replace("<", "&lt;", StringComparison.Ordinal).Replace(">", "&gt;", StringComparison.Ordinal);

    private static void EnsureValid(GraphicDocument document)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));
        var errors = GraphicDocumentValidator.Validate(document).Where(diagnostic => diagnostic.Severity == "Error").ToArray();
        if (errors.Length > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(diagnostic => diagnostic.Code + ": " + diagnostic.Message)));
    }
}
