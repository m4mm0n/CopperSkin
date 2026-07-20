using System.Windows;
using CopperSkin.Core.Graphics;

namespace CopperSkin.Wpf.Drawing;

/// <summary>
/// Performs document-space hit testing for the shared WPF graphics surface.
/// </summary>
public static class GraphicHitTester
{
    /// <summary>
    /// Returns the topmost visible element containing the supplied document-space point.
    /// </summary>
    public static GraphicHitResult? HitTest(GraphicDocument document, Point point)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));

        for (var layerIndex = document.Layers.Count - 1; layerIndex >= 0; layerIndex--)
        {
            var layer = document.Layers[layerIndex];
            if (!layer.IsVisible || layer.IsLocked)
                continue;

            for (var elementIndex = layer.Elements.Count - 1; elementIndex >= 0; elementIndex--)
            {
                var element = layer.Elements[elementIndex];
                if (Contains(element, point))
                    return new GraphicHitResult(element.Id, layer.Id, point);
            }
        }

        return null;
    }

    private static bool Contains(GraphicElement element, Point point)
    {
        var local = ApplyInverseTransform(element.Transform, point);
        return element.Kind switch
        {
            GraphicElementKind.Rectangle or GraphicElementKind.Text => ToRect(element.Geometry.Bounds).Contains(local),
            GraphicElementKind.Ellipse => ContainsEllipse(element.Geometry.Bounds, local),
            GraphicElementKind.Line => ContainsPoints(element.Geometry.Points, local, 1.5),
            GraphicElementKind.Polygon => ContainsPolygon(element.Geometry.Points, local),
            GraphicElementKind.Freehand => ContainsPoints(element.Geometry.Points, local, 2),
            GraphicElementKind.Path => ContainsPath(element.Geometry.Path, local),
            _ => false
        };
    }

    private static bool ContainsEllipse(GraphicRect bounds, Point point)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return false;
        var dx = (point.X - bounds.X - (bounds.Width / 2)) / (bounds.Width / 2);
        var dy = (point.Y - bounds.Y - (bounds.Height / 2)) / (bounds.Height / 2);
        return (dx * dx) + (dy * dy) <= 1;
    }

    private static bool ContainsPath(IReadOnlyList<GraphicPathCommand> path, Point point)
    {
        return ContainsPoints(path.SelectMany(command => command.Kind == GraphicPathCommandKind.CubicTo
            ? new[] { command.Point1, command.Point2, command.Point3 }
            : new[] { command.Point1 }).ToArray(), point, 2);
    }

    private static bool ContainsPoints(IReadOnlyList<GraphicPoint> points, Point point, double tolerance)
    {
        if (points.Count == 0)
            return false;
        var minX = points.Min(p => p.X) - tolerance;
        var maxX = points.Max(p => p.X) + tolerance;
        var minY = points.Min(p => p.Y) - tolerance;
        var maxY = points.Max(p => p.Y) + tolerance;
        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

    private static bool ContainsPolygon(IReadOnlyList<GraphicPoint> points, Point point)
    {
        if (points.Count < 3)
            return false;
        var inside = false;
        for (var i = 0; i < points.Count; i++)
        {
            var j = (i + points.Count - 1) % points.Count;
            var intersects = (points[i].Y > point.Y) != (points[j].Y > point.Y) &&
                point.X < ((points[j].X - points[i].X) * (point.Y - points[i].Y) / (points[j].Y - points[i].Y)) + points[i].X;
            if (intersects)
                inside = !inside;
        }
        return inside;
    }

    private static Point ApplyInverseTransform(GraphicTransform transform, Point point)
    {
        var x = point.X - transform.X;
        var y = point.Y - transform.Y;
        if (transform.Rotation != 0)
        {
            var radians = -transform.Rotation * (Math.PI / 180);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);
            (x, y) = ((x * cos) - (y * sin), (x * sin) + (y * cos));
        }
        return new Point(transform.ScaleX == 0 ? x : x / transform.ScaleX, transform.ScaleY == 0 ? y : y / transform.ScaleY);
    }

    private static Rect ToRect(GraphicRect rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
}

/// <summary>
/// Describes a hit-tested graphics element and its owning layer.
/// </summary>
public sealed record GraphicHitResult(string ElementId, string LayerId, Point DocumentPoint);
