using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CopperSkin.Core.Theming;
using CopperSkin.Core.Graphics;
using CopperSkin.Wpf.Drawing;

namespace CopperSkin.Wpf.Tests;

/// <summary>
/// Verifies shared WPF rendering and hit testing for Core graphics documents.
/// </summary>
public sealed class GraphicRenderingTests
{
    /// <summary>Verifies a document renders visible pixels through the shared renderer.</summary>
    [WpfFact]
    public void RendererDrawsGraphicElements()
    {
        RunOnSta(() =>
        {
            var document = new GraphicDocument
            {
                Id = "render",
                Width = 24,
                Height = 24,
                Layers =
                [
                    new GraphicLayer
                    {
                        Id = "foreground",
                        Name = "Foreground",
                        Elements =
                        [
                            new GraphicElement
                            {
                                Id = "body",
                                Kind = GraphicElementKind.Rectangle,
                                Geometry = new GraphicGeometry { Bounds = new GraphicRect(2, 2, 20, 20) },
                                Style = new GraphicStyle { Fill = new GraphicColor(255, 255, 0, 0) }
                            }
                        ]
                    }
                ]
            };
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
                GraphicRenderer.Render(context, document, DrawingThemeSnapshot.Default);

            var bitmap = new RenderTargetBitmap(24, 24, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var pixels = new byte[24 * 24 * 4];
            bitmap.CopyPixels(pixels, 24 * 4, 0);

            Assert.Contains(pixels, value => value != 0);
        });
    }

    /// <summary>Verifies hit testing returns the topmost visible element.</summary>
    [WpfFact]
    public void HitTesterReturnsTopmostElement()
    {
        RunOnSta(() =>
        {
            var document = new GraphicDocument
            {
                Id = "hit-test",
                Width = 24,
                Height = 24,
                Layers =
                [
                    new GraphicLayer
                    {
                        Id = "foreground",
                        Name = "Foreground",
                        Elements =
                        [
                            new GraphicElement
                            {
                                Id = "bottom",
                                Kind = GraphicElementKind.Rectangle,
                                Geometry = new GraphicGeometry { Bounds = new GraphicRect(2, 2, 20, 20) }
                            },
                            new GraphicElement
                            {
                                Id = "top",
                                Kind = GraphicElementKind.Ellipse,
                                Geometry = new GraphicGeometry { Bounds = new GraphicRect(6, 6, 12, 12) }
                            }
                        ]
                    }
                ]
            };

            var hit = GraphicHitTester.HitTest(document, new Point(12, 12));

            Assert.NotNull(hit);
            Assert.Equal("top", hit!.ElementId);
            Assert.Equal("foreground", hit.LayerId);
        });
    }

    /// <summary>Verifies graphics fill tokens resolve through the active drawing theme.</summary>
    [WpfFact]
    public void RendererResolvesThemeFillToken()
    {
        RunOnSta(() =>
        {
            var resolved = new ResolvedTheme(
                "test",
                "Test",
                new Dictionary<string, string>
                {
                    ["color.accent.primary"] = "#FF00FF00"
                });
            var document = new GraphicDocument
            {
                Id = "token-render",
                Width = 8,
                Height = 8,
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
                                Id = "token-rectangle",
                                Kind = GraphicElementKind.Rectangle,
                                Geometry = new GraphicGeometry { Bounds = new GraphicRect(0, 0, 8, 8) },
                                Style = new GraphicStyle
                                {
                                    Fill = GraphicColor.Transparent,
                                    FillToken = "color.accent.primary"
                                }
                            }
                        ]
                    }
                ]
            };

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
                GraphicRenderer.Render(context, document, DrawingThemeSnapshot.FromTheme(resolved));

            var bitmap = new RenderTargetBitmap(8, 8, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var pixels = new byte[8 * 8 * 4];
            bitmap.CopyPixels(pixels, 8 * 4, 0);
            var center = (4 * 8 + 4) * 4;

            Assert.True(pixels[center + 1] > 200);
            Assert.True(pixels[center + 3] > 200);
        });
    }

    /// <summary>Verifies the canvas reports a zoomed extent for correct horizontal and vertical scrolling.</summary>
    [WpfFact]
    public void CanvasMeasuresZoomedDocumentExtent()
    {
        RunOnSta(() =>
        {
            var canvas = new GraphicCanvas
            {
                Document = new GraphicDocument { Id = "extent", Width = 40, Height = 20 },
                Zoom = 2.5
            };

            canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            Assert.Equal(100, canvas.DesiredSize.Width);
            Assert.Equal(50, canvas.DesiredSize.Height);
        });
    }

    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
            ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
