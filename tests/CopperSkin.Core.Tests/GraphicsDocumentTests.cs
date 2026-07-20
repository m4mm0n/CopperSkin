using CopperSkin.Core.Graphics;
using CopperSkin.Core.Theming;

namespace CopperSkin.Core.Tests;

/// <summary>
/// Verifies the v0.3 graphics document contract and its theme-pack integration.
/// </summary>
public sealed class GraphicsDocumentTests
{
    /// <summary>Verifies every v0.3 element kind survives serialization.</summary>
    [Fact]
    public void RoundTripPreservesAllV03ElementKinds()
    {
        var document = CreateDocument();

        var json = GraphicDocumentSerializer.Serialize(document);
        var roundTrip = GraphicDocumentSerializer.Deserialize(json);

        Assert.Equal(json, GraphicDocumentSerializer.Serialize(roundTrip));
        Assert.Equal(7, roundTrip.Layers[0].Elements.Count);
        Assert.Equal(GraphicElementKind.Freehand, roundTrip.Layers[0].Elements[5].Kind);
        Assert.Equal("CopperSkin", roundTrip.Layers[0].Elements[6].Text);
    }

    /// <summary>Verifies repeated serialization produces identical JSON.</summary>
    [Fact]
    public void SerializerIsDeterministicForTheSameDocument()
    {
        var document = CreateDocument();

        Assert.Equal(GraphicDocumentSerializer.Serialize(document), GraphicDocumentSerializer.Serialize(document));
    }

    /// <summary>Verifies malformed geometry and style values produce diagnostics.</summary>
    [Fact]
    public void ValidatorReportsMalformedGeometryAndStyle()
    {
        var document = new GraphicDocument
        {
            Id = string.Empty,
            Width = 0,
            Height = double.NaN,
            Layers =
            [
                new GraphicLayer
                {
                    Id = "duplicate",
                    Elements =
                    [
                        new GraphicElement
                        {
                            Id = "shape",
                            Kind = GraphicElementKind.Rectangle,
                            Geometry = new GraphicGeometry { Bounds = new GraphicRect(0, 0, 0, 0) },
                            Style = new GraphicStyle { Opacity = 2, StrokeWidth = -1 }
                        },
                        new GraphicElement { Id = "shape", Kind = GraphicElementKind.Line }
                    ]
                }
            ]
        };

        var diagnostics = GraphicDocumentValidator.Validate(document);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC001");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC002");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC004");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC006");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC007");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC009");
        Assert.Contains(diagnostics, diagnostic => diagnostic.Code == "GRAPHIC010");
    }

    /// <summary>Verifies a future schema version is rejected explicitly.</summary>
    [Fact]
    public void DeserializerRejectsFutureSchemaVersions()
    {
        var json = "{\"schemaVersion\":999,\"id\":\"future\",\"width\":24,\"height\":24,\"layers\":[]}";

        var exception = Assert.Throws<GraphicSchemaException>(() => GraphicDocumentSerializer.Deserialize(json));

        Assert.Equal(999, exception.SchemaVersion);
    }

    /// <summary>Verifies legacy packs do not gain an empty graphics property.</summary>
    [Fact]
    public void LegacyThemePackJsonOmitsEmptyGraphicsSection()
    {
        var path = Path.Combine(Path.GetTempPath(), "copperskin-legacy-" + Guid.NewGuid().ToString("N") + ".json");
        try
        {
            ThemeJsonSerializer.WritePack(path, new ThemePack { Id = "legacy", Name = "Legacy", Version = "0.2.0.0" });

            var json = File.ReadAllText(path);

            Assert.DoesNotContain("graphics", json, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    /// <summary>Verifies graphics changes invalidate a signed pack.</summary>
    [Fact]
    public void GraphicsParticipateInPackSignature()
    {
        var pack = new ThemePack
        {
            Id = "graphics",
            Name = "Graphics",
            Version = "0.3.0.0",
            Graphics = [CreateDocument()]
        };
        var keyPair = ThemePackSigner.CreateKeyPair();

        ThemePackSigner.Sign(pack, "test", keyPair.PrivateKeyHex);
        pack.Graphics![0].Layers[0].Elements[0].Style.Opacity = 0.25;

        Assert.False(ThemePackSigner.Verify(pack, keyPair.PublicKeyHex));
    }

    private static GraphicDocument CreateDocument()
    {
        return new GraphicDocument
        {
            Id = "sample-icon",
            Name = "Sample Icon",
            DocumentType = GraphicDocumentType.Icon,
            Width = 24,
            Height = 24,
            Background = GraphicColor.Transparent,
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
                            Id = "path",
                            Kind = GraphicElementKind.Path,
                            Geometry = new GraphicGeometry
                            {
                                Path =
                                [
                                    new GraphicPathCommand { Kind = GraphicPathCommandKind.MoveTo, Point1 = new(1, 1) },
                                    new GraphicPathCommand { Kind = GraphicPathCommandKind.LineTo, Point1 = new(23, 1) },
                                    new GraphicPathCommand { Kind = GraphicPathCommandKind.Close }
                                ]
                            }
                        },
                        new GraphicElement
                        {
                            Id = "line",
                            Kind = GraphicElementKind.Line,
                            Geometry = new GraphicGeometry { Points = [new(1, 1), new(23, 23)] }
                        },
                        new GraphicElement
                        {
                            Id = "rectangle",
                            Kind = GraphicElementKind.Rectangle,
                            Geometry = new GraphicGeometry { Bounds = new GraphicRect(2, 2, 20, 20) }
                        },
                        new GraphicElement
                        {
                            Id = "ellipse",
                            Kind = GraphicElementKind.Ellipse,
                            Geometry = new GraphicGeometry { Bounds = new GraphicRect(4, 4, 16, 16) }
                        },
                        new GraphicElement
                        {
                            Id = "polygon",
                            Kind = GraphicElementKind.Polygon,
                            Geometry = new GraphicGeometry { Points = [new(12, 1), new(23, 23), new(1, 23)] }
                        },
                        new GraphicElement
                        {
                            Id = "freehand",
                            Kind = GraphicElementKind.Freehand,
                            Geometry = new GraphicGeometry { Points = [new(3, 3), new(5, 5), new(7, 4)] }
                        },
                        new GraphicElement
                        {
                            Id = "text",
                            Kind = GraphicElementKind.Text,
                            Text = "CopperSkin",
                            Geometry = new GraphicGeometry { Bounds = new GraphicRect(2, 8, 20, 8) }
                        }
                    ]
                }
            ]
        };
    }
}
