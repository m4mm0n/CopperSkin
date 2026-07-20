using System.IO;
using System.Windows.Media.Imaging;
using CopperSkin.Core.Graphics;
using CopperSkin.Wpf.Drawing;

namespace CopperSkin.Wpf.Tests;

/// <summary>
/// Verifies deterministic vector and raster graphics export.
/// </summary>
public sealed class GraphicExportTests
{
    /// <summary>Verifies SVG and XAML exports are stable and contain the expected drawing types.</summary>
    [WpfFact]
    public void VectorExportsAreDeterministic()
    {
        var document = CreateDocument();

        var svg = GraphicExportService.ToSvg(document);
        var xaml = GraphicExportService.ToXaml(document);

        Assert.Equal(svg, GraphicExportService.ToSvg(document));
        Assert.Equal(xaml, GraphicExportService.ToXaml(document));
        Assert.Contains("<svg", svg, StringComparison.Ordinal);
        Assert.Contains("<rect", svg, StringComparison.Ordinal);
        Assert.Contains("DrawingImage", xaml, StringComparison.Ordinal);
        Assert.Contains("RectangleGeometry", xaml, StringComparison.Ordinal);
    }

    /// <summary>Verifies PNG export creates a readable image of the requested dimensions.</summary>
    [WpfFact]
    public void PngExportCreatesRequestedImage()
    {
        var path = Path.Combine(Path.GetTempPath(), "copperskin-graphic-" + Guid.NewGuid().ToString("N") + ".png");
        try
        {
            GraphicExportService.ExportPng(CreateDocument(), path, 48, 48, DrawingThemeSnapshot.Default);

            using var stream = File.OpenRead(path);
            var frame = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            Assert.Equal(48, frame.PixelWidth);
            Assert.Equal(48, frame.PixelHeight);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private static GraphicDocument CreateDocument() => new()
    {
        Id = "export",
        Width = 24,
        Height = 24,
        Layers =
        [
            new GraphicLayer
            {
                Id = "layer",
                Name = "Layer",
                Elements =
                [
                    new GraphicElement
                    {
                        Id = "rectangle",
                        Kind = GraphicElementKind.Rectangle,
                        Geometry = new GraphicGeometry { Bounds = new GraphicRect(2, 2, 20, 20) },
                        Style = new GraphicStyle { Fill = new GraphicColor(255, 185, 97, 31) }
                    }
                ]
            }
        ]
    };
}
