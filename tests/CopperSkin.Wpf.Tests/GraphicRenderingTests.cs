using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
