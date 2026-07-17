using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CopperSkin.Core.Graphics;
using CopperSkin.Wpf.Drawing;
using Microsoft.Win32;

namespace CopperSkin.Designer.Graphics;

/// <summary>
/// Provides the first interactive CopperSkin icon and basic-paint authoring surface.
/// </summary>
public partial class GraphicsEditorView : UserControl
{
    private readonly GraphicsHistory _history = new();
    private readonly ObservableCollection<GraphicLayer> _layers = new();
    private readonly List<GraphicPoint> _stroke = new();
    private Point? _dragStart;
    private GraphicsEditorTool _tool = GraphicsEditorTool.Select;

    /// <summary>
    /// Initializes the graphics editor with a new icon document.
    /// </summary>
    public GraphicsEditorView()
    {
        InitializeComponent();
        LayerList.ItemsSource = _layers;
        CreateDocument(GraphicDocumentType.Icon);
    }

    private GraphicDocument Document => GraphicSurface.Document!;

    private void CreateDocument(GraphicDocumentType type)
    {
        var document = type == GraphicDocumentType.Icon
            ? new GraphicDocument { Id = "new-icon", Name = "New Icon", DocumentType = type, Width = 64, Height = 64, Background = GraphicColor.Transparent }
            : new GraphicDocument { Id = "new-paint", Name = "New Paint", DocumentType = type, Width = 640, Height = 480, Background = new GraphicColor(255, 36, 27, 50) };
        document.Layers.Add(new GraphicLayer { Id = "layer-1", Name = "Foreground" });
        SetDocument(document, clearHistory: true);
        StatusText.Text = $"Created {document.Name}.";
    }

    private void SetDocument(GraphicDocument document, bool clearHistory)
    {
        GraphicSurface.Document = document;
        if (clearHistory)
            _history.Clear();
        _layers.Clear();
        foreach (var layer in document.Layers)
            _layers.Add(layer);
        DocumentNameText.Text = document.Name;
        DocumentSizeText.Text = $"{document.Width:0} × {document.Height:0} · {document.DocumentType}";
        ToolText.Text = _tool.ToString();
        GraphicSurface.InvalidateVisual();
    }

    private void NewIcon_Click(object sender, RoutedEventArgs e) => CreateDocument(GraphicDocumentType.Icon);

    private void NewPaint_Click(object sender, RoutedEventArgs e) => CreateDocument(GraphicDocumentType.Paint);

    private void Tool_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag } && Enum.TryParse<GraphicsEditorTool>(tag, out var tool))
        {
            _tool = tool;
            ToolText.Text = tool.ToString();
            StatusText.Text = $"Tool: {tool}.";
        }
    }

    private void GraphicSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var point = GraphicSurfaceMousePoint(e);
        if (_tool == GraphicsEditorTool.Select)
        {
            var hit = GraphicHitTester.HitTest(Document, point);
            StatusText.Text = hit is null ? "Nothing selected." : $"Selected {hit.ElementId}.";
            return;
        }

        _dragStart = point;
        _stroke.Clear();
        _stroke.Add(new GraphicPoint(point.X, point.Y));
        GraphicSurface.CaptureMouse();
        e.Handled = true;
    }

    private void GraphicSurface_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is null || _tool != GraphicsEditorTool.Freehand || e.LeftButton != MouseButtonState.Pressed)
            return;

        var point = GraphicSurfaceMousePoint(e);
        if (_stroke.Count == 0 || Distance(_stroke[^1], point) >= 1)
            _stroke.Add(new GraphicPoint(point.X, point.Y));
    }

    private void GraphicSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragStart is null)
            return;

        var end = GraphicSurfaceMousePoint(e);
        var start = _dragStart.Value;
        _dragStart = null;
        GraphicSurface.ReleaseMouseCapture();
        var element = CreateElement(start, end);
        if (element is null)
            return;

        var layer = Document.Layers.FirstOrDefault() ?? AddLayer();
        _history.Record(Document);
        layer.Elements.Add(element);
        LayerList.Items.Refresh();
        GraphicSurface.InvalidateVisual();
        StatusText.Text = $"Added {element.Kind} {element.Id}.";
    }

    private GraphicElement? CreateElement(Point start, Point end)
    {
        var style = new GraphicStyle
        {
            Fill = _tool == GraphicsEditorTool.Freehand || _tool == GraphicsEditorTool.Line ? GraphicColor.Transparent : new GraphicColor(255, 185, 97, 31),
            Stroke = new GraphicColor(255, 255, 244, 220),
            StrokeWidth = 2
        };
        var geometry = new GraphicGeometry();
        GraphicElementKind kind;
        switch (_tool)
        {
            case GraphicsEditorTool.Rectangle:
                kind = GraphicElementKind.Rectangle;
                geometry.Bounds = Bounds(start, end);
                break;
            case GraphicsEditorTool.Ellipse:
                kind = GraphicElementKind.Ellipse;
                geometry.Bounds = Bounds(start, end);
                break;
            case GraphicsEditorTool.Line:
                kind = GraphicElementKind.Line;
                geometry.Points = [new GraphicPoint(start.X, start.Y), new GraphicPoint(end.X, end.Y)];
                break;
            case GraphicsEditorTool.Freehand:
                kind = GraphicElementKind.Freehand;
                if (_stroke.Count < 2)
                    _stroke.Add(new GraphicPoint(end.X, end.Y));
                geometry.Points = [.. _stroke];
                break;
            default:
                return null;
        }

        return new GraphicElement { Id = Guid.NewGuid().ToString("N"), Kind = kind, Geometry = geometry, Style = style };
    }

    private GraphicLayer AddLayer()
    {
        var layer = new GraphicLayer { Id = Guid.NewGuid().ToString("N"), Name = $"Layer {Document.Layers.Count + 1}" };
        Document.Layers.Add(layer);
        _layers.Add(layer);
        return layer;
    }

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        var document = _history.Undo(Document);
        if (document is null)
        {
            StatusText.Text = "Nothing to undo.";
            return;
        }
        SetDocument(document, clearHistory: false);
        StatusText.Text = "Undo applied.";
    }

    private void Redo_Click(object sender, RoutedEventArgs e)
    {
        var document = _history.Redo(Document);
        if (document is null)
        {
            StatusText.Text = "Nothing to redo.";
            return;
        }
        SetDocument(document, clearHistory: false);
        StatusText.Text = "Redo applied.";
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "CopperSkin graphics (*.cgraphic;*.json)|*.cgraphic;*.json|All files (*.*)|*.*" };
        if (dialog.ShowDialog() != true)
            return;
        try
        {
            SetDocument(GraphicDocumentSerializer.Deserialize(File.ReadAllText(dialog.FileName)), clearHistory: true);
            StatusText.Text = $"Opened {dialog.FileName}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Could not open graphics document: {exception.Message}";
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "CopperSkin graphics (*.cgraphic)|*.cgraphic|JSON (*.json)|*.json", FileName = Document.Id + ".cgraphic" };
        if (dialog.ShowDialog() != true)
            return;
        try
        {
            File.WriteAllText(dialog.FileName, GraphicDocumentSerializer.Serialize(Document));
            StatusText.Text = $"Saved {dialog.FileName}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Could not save graphics document: {exception.Message}";
        }
    }

    private void ExportSvg_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "SVG files (*.svg)|*.svg", FileName = Document.Id + ".svg" };
        if (dialog.ShowDialog() != true)
            return;
        try
        {
            File.WriteAllText(dialog.FileName, GraphicExportService.ToSvg(Document));
            StatusText.Text = $"Exported {dialog.FileName}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Could not export SVG: {exception.Message}";
        }
    }

    private void ExportXaml_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "WPF XAML files (*.xaml)|*.xaml", FileName = Document.Id + ".xaml" };
        if (dialog.ShowDialog() != true)
            return;
        try
        {
            File.WriteAllText(dialog.FileName, GraphicExportService.ToXaml(Document));
            StatusText.Text = $"Exported {dialog.FileName}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Could not export XAML: {exception.Message}";
        }
    }

    private void ExportPng_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "PNG files (*.png)|*.png", FileName = Document.Id + ".png" };
        if (dialog.ShowDialog() != true)
            return;
        try
        {
            var width = Math.Max(1, (int)Math.Round(Document.Width));
            var height = Math.Max(1, (int)Math.Round(Document.Height));
            GraphicExportService.ExportPng(Document, dialog.FileName, width, height);
            StatusText.Text = $"Exported {dialog.FileName}.";
        }
        catch (Exception exception)
        {
            StatusText.Text = $"Could not export PNG: {exception.Message}";
        }
    }

    private void ZoomIn_Click(object sender, RoutedEventArgs e) => GraphicSurface.Zoom = Math.Min(8, GraphicSurface.Zoom * 1.25);

    private void ZoomOut_Click(object sender, RoutedEventArgs e) => GraphicSurface.Zoom = Math.Max(0.25, GraphicSurface.Zoom / 1.25);

    private Point GraphicSurfaceMousePoint(MouseEventArgs e)
    {
        var point = e.GetPosition(GraphicSurface);
        var zoom = GraphicSurface.Zoom <= 0 ? 1 : GraphicSurface.Zoom;
        return new Point(point.X / zoom, point.Y / zoom);
    }

    private static double Distance(GraphicPoint point, Point other)
    {
        var dx = point.X - other.X;
        var dy = point.Y - other.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }

    private static GraphicRect Bounds(Point start, Point end) => new(
        Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
}
