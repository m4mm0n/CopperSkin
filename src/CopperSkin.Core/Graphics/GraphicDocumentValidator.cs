namespace CopperSkin.Core.Graphics;

/// <summary>
/// Validates graphics documents without requiring WPF or a desktop runtime.
/// </summary>
public static class GraphicDocumentValidator
{
    /// <summary>
    /// Returns all structural and range diagnostics without mutating the document.
    /// </summary>
    public static IReadOnlyList<GraphicDiagnostic> Validate(GraphicDocument document)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));
        var diagnostics = new List<GraphicDiagnostic>();

        if (document.SchemaVersion <= 0 || document.SchemaVersion > GraphicDocument.CurrentSchemaVersion)
            diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC003", "The graphics schema version is unsupported.", "schemaVersion"));
        if (string.IsNullOrWhiteSpace(document.Id))
            diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC001", "A graphics document id is required.", "id"));
        if (!IsPositiveFinite(document.Width) || !IsPositiveFinite(document.Height))
            diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC002", "Canvas width and height must be finite and greater than zero.", "canvas"));

        var elementIds = new HashSet<string>(StringComparer.Ordinal);
        for (var layerIndex = 0; layerIndex < document.Layers.Count; layerIndex++)
        {
            var layer = document.Layers[layerIndex];
            var layerPath = $"layers/{layerIndex}";
            if (string.IsNullOrWhiteSpace(layer.Name))
                diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC004", "A graphics layer name is required.", $"{layerPath}/name"));
            if (string.IsNullOrWhiteSpace(layer.Id))
                diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC005", "A graphics layer id is required.", $"{layerPath}/id"));

            for (var elementIndex = 0; elementIndex < layer.Elements.Count; elementIndex++)
            {
                var element = layer.Elements[elementIndex];
                var path = $"{layerPath}/elements/{elementIndex}";
                if (string.IsNullOrWhiteSpace(element.Id) || !elementIds.Add(element.Id))
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC006", "Element ids must be non-empty and unique.", $"{path}/id"));

                ValidateGeometry(element, diagnostics, path);
                if (!IsUnitInterval(element.Style.Opacity))
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC009", "Element opacity must be from zero through one.", $"{path}/style/opacity"));
                if (!IsFinite(element.Style.StrokeWidth) || element.Style.StrokeWidth < 0)
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC010", "Stroke width must be finite and non-negative.", $"{path}/style/strokeWidth"));
            }
        }

        return diagnostics;
    }

    private static void ValidateGeometry(GraphicElement element, List<GraphicDiagnostic> diagnostics, string path)
    {
        var geometry = element.Geometry;
        switch (element.Kind)
        {
            case GraphicElementKind.Rectangle:
            case GraphicElementKind.Ellipse:
            case GraphicElementKind.Text:
                if (!IsPositiveFinite(geometry.Bounds.Width) || !IsPositiveFinite(geometry.Bounds.Height))
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC007", "Bounds must have finite positive width and height.", $"{path}/geometry/bounds"));
                if (element.Kind == GraphicElementKind.Text && string.IsNullOrWhiteSpace(element.Text))
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "Text elements require text content.", $"{path}/text"));
                break;
            case GraphicElementKind.Line:
                if (geometry.Points.Count != 2)
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "Line elements require exactly two points.", $"{path}/geometry/points"));
                break;
            case GraphicElementKind.Polygon:
                if (geometry.Points.Count < 3)
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "Polygon elements require at least three points.", $"{path}/geometry/points"));
                break;
            case GraphicElementKind.Freehand:
                if (geometry.Points.Count < 2)
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "Freehand elements require at least two points.", $"{path}/geometry/points"));
                break;
            case GraphicElementKind.Path:
                if (geometry.Path.Count == 0)
                    diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "Path elements require at least one command.", $"{path}/geometry/path"));
                break;
            default:
                diagnostics.Add(new GraphicDiagnostic("Error", "GRAPHIC008", "The element kind is unsupported.", $"{path}/kind"));
                break;
        }
    }

    private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    private static bool IsPositiveFinite(double value) => IsFinite(value) && value > 0;

    private static bool IsUnitInterval(double value) => IsFinite(value) && value >= 0 && value <= 1;
}

/// <summary>
/// Represents one graphics-document validation diagnostic.
/// </summary>
public sealed record GraphicDiagnostic(string Severity, string Code, string Message, string Path);
